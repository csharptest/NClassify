namespace NClassify.Library
{
    class XmlReading
    {
        public static void ReadXml(string localName, global::System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            if (!reader.IsStartElement(localName))
                throw new global::System.FormatException();

            if (reader.MoveToFirstAttribute())
                MergeFrom(reader);

            bool empty = reader.IsEmptyElement;
            reader.ReadStartElement(localName);
            if(!empty)
            {
                MergeFrom(reader);
                reader.ReadEndElement();
            }
        }
        public static void MergeFrom(global::System.Xml.XmlReader reader)
        {
            int depth = reader.Depth;
            string[] fields = new string[] { "callback", "callbacks" };
            bool[] isMessage = new bool[] { true, true };
            while (!reader.EOF && reader.Depth >= depth)
            {
                bool isElement = reader.NodeType == global::System.Xml.XmlNodeType.Element;
                bool isAttribute = reader.NodeType == global::System.Xml.XmlNodeType.Attribute;
                if(!isElement && !isAttribute)
                {
                    reader.Read();
                    continue;
                }

                int field = global::System.Array.BinarySearch(fields, reader.LocalName);
                if (isElement && field >= 0 && isMessage[field])
                {
                    ReadXml(reader.LocalName, reader);
                }
                else
                {
                    global::System.Text.StringBuilder value = new global::System.Text.StringBuilder();
                    string name = reader.LocalName;
                    if(isAttribute)
                    {
                        value.Append(reader.Value);
                        if (!reader.MoveToNextAttribute())
                            reader.MoveToElement();
                    }
                    else
                    {
                        int stop = reader.Depth;
                        while (reader.Read() && reader.Depth > stop)
                        {
                            while (reader.IsStartElement())
                                reader.Skip();
                            if(((1 << (int)reader.NodeType) & 0x6018) != 0)
                                value.Append(reader.Value);
                        }
                    }

                    global::System.Console.WriteLine("{0} = {1}", name, value);
                }
            }
        }
    }
}
