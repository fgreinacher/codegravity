using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using nsplit.DataStructures;

namespace nsplit.Api
{
    public class TreeViewController : ApiController
    {
        public IEnumerable<QualifiedTreeNodeDto> GetChildren(string id)
        {
            int idNo = 0;
            idNo = id == "#" ? 0 : int.Parse(id);

            IQualifiedTreeNode parent;
            bool isOk = GetTree().TryGet(idNo, out parent);
            if (!isOk) return Enumerable.Empty<QualifiedTreeNodeDto>();
            return parent.Children().Select(ToDto);
        }

        private QualifiedTree GetTree()
        {
            return Program.TypeTree;
        }

        private static QualifiedTreeNodeDto ToDto(IQualifiedTreeNode node)
        {
            return new QualifiedTreeNodeDto
            {
                Id = node.Id,
                Text = node.Name,
                HasChildren = !node.IsLeaf(),
                Icon = node.IsLeaf() ? "/css/c.png" : "/css/n.png"
            };
        }
    }
}