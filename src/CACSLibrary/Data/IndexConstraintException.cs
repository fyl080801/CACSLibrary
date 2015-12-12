namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexConstraintException : CACSException
    {
        private string[] _fields;

        /// <summary>
        /// 
        /// </summary>
        public string[] Fields
        {
            get { return _fields ?? (_fields = new string[0]); }
            set { this._fields = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get { return string.Format("字段 {0} 在集合中重复", string.Join(",", this.Fields)); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        public IndexConstraintException(params string[] fields)
        {
            this._fields = fields;
        }
    }
}
