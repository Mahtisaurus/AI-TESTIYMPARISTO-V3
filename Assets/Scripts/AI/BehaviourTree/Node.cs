using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using BehaviourTrees;
using System.Text;

namespace BehaviourTrees
{
    public class BehaviourTree : Node
    {
        readonly IPolicy policy;

        public BehaviourTree(string name, IPolicy policy = null) : base(name)
        {
            this.policy = policy ?? Policies.RunForever;
        }

        public override Status Process()
        {
            Status status = children[currentChild].Process();
            if (policy.ShouldReturn(status))
            {
                return status;
            }

            currentChild = (currentChild + 1) % children.Count;
            return Status.Running;
        }

        public void PrintTree()
        {
            StringBuilder sb = new StringBuilder();
            PrintNode(this, 0, sb);
            Debug.Log(sb.ToString());
        }

        static void PrintNode(Node node, int indentLevel, StringBuilder sb)
        {
            sb.Append(' ', indentLevel * 2).AppendLine(node.name);
            foreach (Node child in node.children)
            {
                PrintNode(child, indentLevel + 1, sb);
            }
        }
    }

    //public BehaviourTree(string name) : base(name) { }

    //// Iterate over all of the children and run their process methods
    //public override Status Process()
    //{
    //    while(currentChild < children.Count)
    //    {
    //        var status = children[currentChild].Process();

    //        if(status != Status.Success)
    //        {
    //            return status;
    //        }

    //        currentChild++;
    //    }
    //    // Processed all the children and they all returned success
    //    return Status.Success;
    //}


    public class Node
    {
        #region VARIABLES

        // Node status
        public enum Status { Success, Failure, Running }

        public readonly string name;
        public readonly int priority;

        public readonly List<Node> children = new();
        protected int currentChild;

        #endregion

        // Priority determines the nodes priority in execution. Default = 0
        public Node(string name = "Node", int priority = 0)
        {
            this.name = name;
            this.priority = priority;
        }

        // Adds child to node
        public void AddChild(Node child) => children.Add(child);

        // Calling child and run its process
        public virtual Status Process() => children[currentChild].Process();

        // Reset entire node
        public virtual void Reset()
        {
            currentChild = 0;
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }

    // Behaviour to execute
    public class Leaf : Node
    {
        readonly IStrategy strategy;

        // Constructor
        public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)
        {
            this.strategy = strategy;
        }

        // Execute the strategy
        public override Status Process() => strategy.Process();
        // Reset strategy
        public override void Reset() => strategy.Reset();
    }

    // Goes through all of its childs and executes them one by one until all of them have succeeded. Logical AND
    public class Sequence : Node
    {
        public Sequence(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            if(currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        currentChild = 0;
                        //Reset();
                        return Status.Failure;
                    default:
                        currentChild++;
                        return currentChild == children.Count ? Status.Success : Status.Running;
                }
            }

            Reset();
            return Status.Success;
        }
    }

    // Logical OR. If any of the children succeed = success
    public class Selector : Node
    {
        public Selector(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            if(currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    default:
                        currentChild++;
                        return Status.Running;
                }
            }

            Reset();
            return Status.Failure;
        }
    }

    // Selector but with sorting the priorities of the children and executing in order
    public class PrioritySelector : Selector
    {
        List<Node> sortedChildren;
        List<Node> SortedChildren => sortedChildren ??= SortChildren();

        protected virtual List<Node> SortChildren() => children.OrderByDescending(child => child.priority).ToList();

        public PrioritySelector(string name, int priority = 0) : base(name, priority) { }

        public override void Reset()
        {
            base.Reset();
            sortedChildren = null;
        }

        public override Status Process()
        {
            foreach (var child in SortedChildren)
            {
                switch (child.Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    default:
                        continue;
                }
            }

            Reset();
            return Status.Failure;
        }
    }

    // randomly selecting the priority
    public class RandomSelector : PrioritySelector
    {
        protected override List<Node> SortChildren() => children.Shuffle().ToList();

        public RandomSelector(string name, int priority = 0) : base(name, priority) { }
    }

    // Inverts the outcome
    public class Inverter : Node
    {
        public Inverter(string name) : base(name) { }

        public override Status Process()
        {

            switch (children[0].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    return Status.Success;
                default:
                    return Status.Failure;
            }
        }
    }

    // Runs forever or until failure
    public class UntilFail : Node
    {
        public UntilFail(string name) : base(name) { }

        public override Status Process()
        {
            if(children[0].Process() == Status.Failure)
            {
                Reset();
                return Status.Failure;
            }

            return Status.Running;

        }
    }

    // Absolutely runs as long as its getting failures
    public class UntilSuccess : Node
    {
        public UntilSuccess(string name) : base(name) { }

        public override Status Process()
        {
            if (children[0].Process() == Status.Success)
            {
                Reset();
                return Status.Success;
            }

            return Status.Failure;

        }
    }

    // Could also add a node that runs on repeat for x times?

    // Shuffling a list
    public static class ListExtensions
    {
        static System.Random rng;

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            if (rng == null) rng = new System.Random();
            int count = list.Count;
            while(count > 1) 
            {
                --count;
                int index = rng.Next(count + 1);
                (list[index], list[count]) = (list[count], list[index]);
            }

            return list;
        }
    }

    public interface IPolicy
    {
        bool ShouldReturn(Node.Status status);
    }

    public static class Policies
    {
        public static readonly IPolicy RunForever = new RunForeverPolicy();
        public static readonly IPolicy RunUntilSuccess = new RunUntilSuccessPolicy();
        public static readonly IPolicy RunUntilFailure = new RunUntilFailurePolicy();

        class RunForeverPolicy : IPolicy
        {
            public bool ShouldReturn(Node.Status status) => false;
        }

        class RunUntilSuccessPolicy : IPolicy
        {
            public bool ShouldReturn(Node.Status status) => status == Node.Status.Success;
        }

        class RunUntilFailurePolicy : IPolicy
        {
            public bool ShouldReturn(Node.Status status) => status == Node.Status.Failure;
        }
    }

} // namespace END