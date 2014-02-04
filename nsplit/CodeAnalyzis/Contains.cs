using System;

namespace nsplit.CodeAnalyzis
{
    public class Contains : Dependency
    {
        public Contains(Type source, Type nested) 
            : base(source, nested)
        {
            
        }

        public override DependencyKind Kind
        {
            get { return DependencyKind.Contains;}
        }
    }
}