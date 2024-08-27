namespace AHI.Infrastructure.Service.Tag.Model
{
    public class SelectTableInfo
    {
        public SelectTableInfo() { }
        public SelectTableInfo(string entityTableName, string selectEntityWildcard, string entityTablePrimaryColumn, string entityType)
        {
            _entityTableName = entityTableName;
            _selectEntityWildcard = selectEntityWildcard;
            _entityTablePrimaryColumn = entityTablePrimaryColumn;
            _entityType = entityType;
        }

        private string _entityTableName;
        /// <summary>
        /// Main table name
        /// </summary>
        public string EntityTableName
        {
            get { return _entityTableName.Replace("'", "''"); }
            set { _entityTableName = value; }
        }

        private string _entityTablePrimaryColumn;

        /// <summary>
        /// Main table primary column, usually Id
        /// </summary>
        public string EntityTablePrimaryColumn
        {
            get { return _entityTablePrimaryColumn.Replace("'", "''"); }
            set { _entityTablePrimaryColumn = value; }
        }

        private string _selectEntityWildcard;

        /// <summary>
        /// Main table wildcard ex: select * from tableA tba that "tba" is wildcard
        /// </summary>
        public string SelectEntityWildcard
        {
            get { return _selectEntityWildcard.Replace("'", "''"); }
            set { _selectEntityWildcard = value; }
        }

        private string _entityType;

        /// <summary>
        /// Type of entity id
        /// </summary>
        public string EntityType
        {
            get { return _entityType.Replace("'", "''"); }
            set { _entityType = value; }
        }
    }
}