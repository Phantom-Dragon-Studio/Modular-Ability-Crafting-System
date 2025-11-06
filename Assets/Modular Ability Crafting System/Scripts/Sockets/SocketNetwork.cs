using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.Runes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Sockets
{
    /// <summary>
    /// Manages a network of sockets for spell progression, talent trees, or spell unlocking.
    /// Can be used to create skill trees where unlocking one spell/rune unlocks others.
    /// </summary>
    public class SocketNetwork : MonoBehaviour
    {
        [Header("Network Configuration")]
        [Tooltip("All nodes in this network")]
        [SerializeField] private List<UnlockNode> nodes = new List<UnlockNode>();

        [Tooltip("Sockets managed by this network")]
        [SerializeField] private List<AbilitySocket> sockets = new List<AbilitySocket>();

        [Header("Progression")]
        [Tooltip("Points available for unlocking nodes")]
        [SerializeField] private int availablePoints = 0;

        [Tooltip("Total points earned")]
        [SerializeField] private int totalPoints = 0;

        // Events
        public event Action<UnlockNode> OnNodeUnlocked;
        public event Action<UnlockNode> OnNodeLocked;
        public event Action<int> OnPointsChanged;

        // Public Properties
        public List<UnlockNode> Nodes => new List<UnlockNode>(nodes);
        public List<AbilitySocket> Sockets => new List<AbilitySocket>(sockets);
        public int AvailablePoints => availablePoints;
        public int TotalPoints => totalPoints;

        /// <summary>
        /// Add points to the network
        /// </summary>
        public void AddPoints(int points)
        {
            if (points <= 0) return;

            availablePoints += points;
            totalPoints += points;
            OnPointsChanged?.Invoke(availablePoints);
        }

        /// <summary>
        /// Try to unlock a node
        /// </summary>
        public bool UnlockNode(UnlockNode node)
        {
            if (node == null) return false;

            // Check if already unlocked
            if (node.IsUnlocked)
            {
                Debug.LogWarning($"Node '{node.NodeName}' is already unlocked");
                return false;
            }

            // Check point cost
            if (availablePoints < node.PointCost)
            {
                Debug.LogWarning($"Not enough points to unlock '{node.NodeName}' (need {node.PointCost}, have {availablePoints})");
                return false;
            }

            // Check prerequisites
            if (!ArePrerequisitesMet(node))
            {
                Debug.LogWarning($"Prerequisites not met for '{node.NodeName}'");
                return false;
            }

            // Unlock the node
            availablePoints -= node.PointCost;
            node.Unlock();
            OnNodeUnlocked?.Invoke(node);
            OnPointsChanged?.Invoke(availablePoints);

            return true;
        }

        /// <summary>
        /// Lock a node (refund points)
        /// </summary>
        public bool LockNode(UnlockNode node, bool allowRefund = true)
        {
            if (node == null) return false;

            if (!node.IsUnlocked)
            {
                Debug.LogWarning($"Node '{node.NodeName}' is not unlocked");
                return false;
            }

            // Check if any other nodes depend on this one
            foreach (var other in nodes)
            {
                if (other.IsUnlocked && other.Prerequisites != null && other.Prerequisites.Contains(node))
                {
                    Debug.LogWarning($"Cannot lock '{node.NodeName}' - other nodes depend on it");
                    return false;
                }
            }

            // Lock the node
            node.Lock();

            if (allowRefund)
            {
                availablePoints += node.PointCost;
            }

            OnNodeLocked?.Invoke(node);
            OnPointsChanged?.Invoke(availablePoints);

            return true;
        }

        /// <summary>
        /// Check if a node's prerequisites are met
        /// </summary>
        public bool ArePrerequisitesMet(UnlockNode node)
        {
            if (node == null) return false;
            if (node.Prerequisites == null || node.Prerequisites.Length == 0) return true;

            foreach (var prereq in node.Prerequisites)
            {
                if (prereq == null || !prereq.IsUnlocked)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get all unlocked nodes
        /// </summary>
        public List<UnlockNode> GetUnlockedNodes()
        {
            return nodes.Where(n => n.IsUnlocked).ToList();
        }

        /// <summary>
        /// Get all locked nodes
        /// </summary>
        public List<UnlockNode> GetLockedNodes()
        {
            return nodes.Where(n => !n.IsUnlocked).ToList();
        }

        /// <summary>
        /// Get all nodes that can currently be unlocked
        /// </summary>
        public List<UnlockNode> GetAvailableNodes()
        {
            return nodes.Where(n => !n.IsUnlocked &&
                                   ArePrerequisitesMet(n) &&
                                   n.PointCost <= availablePoints).ToList();
        }

        /// <summary>
        /// Reset the entire network (lock all nodes, refund all points)
        /// </summary>
        public void ResetNetwork(bool keepTotalPoints = true)
        {
            foreach (var node in nodes)
            {
                if (node.IsUnlocked)
                {
                    node.Lock();
                }
            }

            if (keepTotalPoints)
            {
                availablePoints = totalPoints;
            }
            else
            {
                availablePoints = 0;
                totalPoints = 0;
            }

            OnPointsChanged?.Invoke(availablePoints);
        }

        /// <summary>
        /// Add a node to the network
        /// </summary>
        public void AddNode(UnlockNode node)
        {
            if (node != null && !nodes.Contains(node))
            {
                nodes.Add(node);
            }
        }

        /// <summary>
        /// Remove a node from the network
        /// </summary>
        public void RemoveNode(UnlockNode node)
        {
            nodes.Remove(node);
        }

        /// <summary>
        /// Add a socket to the network
        /// </summary>
        public void AddSocket(AbilitySocket socket)
        {
            if (socket != null && !sockets.Contains(socket))
            {
                sockets.Add(socket);
            }
        }

        /// <summary>
        /// Remove a socket from the network
        /// </summary>
        public void RemoveSocket(AbilitySocket socket)
        {
            sockets.Remove(socket);
        }
    }

    /// <summary>
    /// Represents a node in the unlock network (spell, rune, or ability)
    /// </summary>
    [System.Serializable]
    public class UnlockNode
    {
        [Header("Node Identity")]
        [Tooltip("Name of this node")]
        public string NodeName = "Unlock Node";

        [Tooltip("Description of what this unlocks")]
        [TextArea(2, 4)]
        public string Description;

        [Tooltip("Icon for UI")]
        public Sprite Icon;

        [Header("Unlock Type")]
        [Tooltip("What type of unlock is this?")]
        public UnlockType Type = UnlockType.Spell;

        [Tooltip("Spell to unlock (if type is Spell)")]
        public SpellTemplate SpellToUnlock;

        [Tooltip("Rune to unlock (if type is Rune)")]
        public BaseRune RuneToUnlock;

        [Tooltip("Socket to unlock (if type is Socket)")]
        public AbilitySocket SocketToUnlock;

        [Header("Requirements")]
        [Tooltip("Point cost to unlock this node")]
        public int PointCost = 1;

        [Tooltip("Minimum level required")]
        public int MinimumLevel = 0;

        [Tooltip("Prerequisite nodes that must be unlocked first")]
        public UnlockNode[] Prerequisites;

        [Header("State")]
        [Tooltip("Is this node currently unlocked?")]
        public bool IsUnlocked = false;

        /// <summary>
        /// Unlock this node
        /// </summary>
        public void Unlock()
        {
            IsUnlocked = true;
        }

        /// <summary>
        /// Lock this node
        /// </summary>
        public void Lock()
        {
            IsUnlocked = false;
        }

        public enum UnlockType
        {
            Spell,      // Unlocks a spell template
            Rune,       // Unlocks a rune for crafting
            Socket,     // Unlocks a spell slot
            Passive     // Unlocks a passive ability/bonus
        }
    }
}

