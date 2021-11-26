using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using RSG;
using static CsPreconditions.Preconditions;

namespace Drone.Descriptor.Loader.Interfaces
{
    public abstract class AbstractDescriptorLoader : IDescriptorLoader
    {
        private const string ID_ATTRIBUTE = "id";
        private readonly Dictionary<string, Type> _collections = new Dictionary<string, Type>();
        private readonly Dictionary<string, Type> _descriptors = new Dictionary<string, Type>();

        public IDescriptorLoader AddCollection<T>(string collectionName)
        {
            _collections.Add(collectionName, typeof(T));
            return this;
        }

        public IDescriptorLoader AddDescriptor<T>(string descriptorName)
        {
            _descriptors.Add(descriptorName, typeof(T));
            return this;
        }

        public abstract IPromise Load();

        protected void FillCollection(string text, Type descriptorType, IDescriptorCollection descriptorCollection)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            CheckNotNull(doc.DocumentElement);
            XmlNodeList xmlNodeList = doc.DocumentElement.ChildNodes;

            Dictionary<string, object> map = new Dictionary<string, object>();

            for (int i = 0; i < xmlNodeList.Count; i++) {
                XmlNode currentXmlNode = xmlNodeList.Item(i);
                string id = currentXmlNode.GetStringAttribute(ID_ATTRIBUTE);
                string id = currentXmlNode.

                XmlSerializer deserializer = new XmlSerializer(descriptorType);
                using (TextReader textReader = new StringReader(currentXmlNode.OuterXml)) {
                    map[id] = deserializer.Deserialize(textReader);
                }
            }

            descriptorCollection?.Clear();
            descriptorCollection?.PutAll(map);
        }

        protected object CreateSingleDescriptor(string text, Type descriptorValue)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            CheckNotNull(doc.DocumentElement);

            XmlSerializer xmlSerializer = new XmlSerializer(descriptorValue);
            using (TextReader textReader = new StringReader(doc.OuterXml)) {
                return xmlSerializer.Deserialize(textReader);
            }
        }

        protected Dictionary<string, Type> Collections
        {
            get { return _collections; }
        }
        protected Dictionary<string, Type> Descriptors
        {
            get { return _descriptors; }
        }
    }
}