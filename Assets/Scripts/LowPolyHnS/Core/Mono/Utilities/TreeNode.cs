using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LowPolyHnS.DataStructures
{
    public class TreeNode<T> : IEnumerable<TreeNode<T>>
    {
        private readonly Dictionary<string, TreeNode<T>> children = new Dictionary<string, TreeNode<T>>();

        private readonly string id;
        private TreeNode<T> parent;
        private T data;

        // INITIALIZE: -----------------------------------------------------------------------------------------------------

        public TreeNode(string id, T data)
        {
            this.id = id;
            this.data = data;
        }

        // PUBLIC METHODS: -------------------------------------------------------------------------------------------------

        public string GetID()
        {
            return id;
        }

        public T GetData()
        {
            return data;
        }

        public void SetData(T data)
        {
            this.data = data;
        }

        public TreeNode<T> GetChild(string id)
        {
            return children[id];
        }

        public bool HasChild(string id)
        {
            return children.ContainsKey(id);
        }

        public TreeNode<T> AddChild(TreeNode<T> item)
        {
            if (item.parent != null)
            {
                item.parent.children.Remove(item.id);
            }

            item.parent = this;
            children.Add(item.id, item);
            return GetChild(item.id);
        }

        // STRING METHODS: ---------------------------------------------------------------------------------------------

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            BuildString(sb, this, 0);

            return sb.ToString();
        }

        public static string BuildString(TreeNode<T> tree)
        {
            StringBuilder sb = new StringBuilder();

            BuildString(sb, tree, 0);

            return sb.ToString();
        }

        private static void BuildString(StringBuilder sb, TreeNode<T> node, int depth)
        {
            sb.AppendLine(node.id.PadLeft(node.id.Length + depth));

            foreach (TreeNode<T> child in node)
            {
                BuildString(sb, child, depth + 1);
            }
        }

        // ENUMERATOR METHODS: -----------------------------------------------------------------------------------------

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            return children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}