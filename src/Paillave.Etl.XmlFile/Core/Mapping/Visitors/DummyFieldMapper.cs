namespace Paillave.Etl.XmlFile.Core.Mapping.Visitors
{
    public class DummyFieldMapper : IXmlFieldMapper
    {
        public XmlFieldDefinition MappingSetter { get; } = new XmlFieldDefinition();

        public T ToXPathQuery<T>(string xPathQuery)
        {
            this.MappingSetter.NodePath = xPathQuery;
            return default;
        }

        public T ToXPathQuery<T>(string xPathQuery, int depthScope)
        {
            this.MappingSetter.NodePath = xPathQuery;
            this.MappingSetter.DepthScope = depthScope;
            return default;
        }
    }
}