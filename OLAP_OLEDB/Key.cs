namespace OLAP_OLEDB
{
    internal class Key
    {
        public string KeyValue { get; set; }
        public bool IsFact { get; set; }
        public string KeyFilter { get; internal set; }
    }
}