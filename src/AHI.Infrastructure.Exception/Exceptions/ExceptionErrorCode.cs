namespace AHI.Infrastructure.Exception
{
    public static class ExceptionErrorCode
    {
        public const string ERROR_ENTITY_NOT_FOUND = "ERROR.ENTITY.NOT_FOUND";
        public const string ERROR_ENTITY_NOT_LOCK = "ERROR.ENTITY.NOT_LOCK";
        public const string SECURITY_VIOLATION = "SECURITY.VIOLATION";
        public const string ENTITY_CIRCULAR_REFERENCE = "ERROR.ENTITY.CIRCULAR_REFERENCE";
        public const string ERROR_ENTITY_DUPLICATION = "ERROR.ENTITY.DUPLICATION";
        public const string ERROR_ENTITY_INVALID = "ERROR.ENTITY.INVALID";
        public const string ERROR_ENTITY_LOCK_ALREADY_LOCKED = "ERROR.ENTITY.LOCK.ALREADY_LOCKED";
        public const string ERROR_ENTITY_LOCK_NOT_LOCKED = "ERROR.ENTITY.LOCK.NOT_LOCKED";
        public const string ERROR_ENTITY_LOCK_REQUESTED_TAKEN_LOCK_BY_OTHER = "ERROR.ENTITY.LOCK.REQUESTED_TAKEN_LOCK_BY_OTHER";
        public const string ERROR_ENTITY_LOCK_REQUESTED_IN_PROCESS = "ERROR.ENTITY.LOCK.REQUESTED_IN_PROCESS";
        public const string ERROR_ENTITY_LOCK_LOCKED_BY_OTHER = "ERROR.ENTITY.LOCK.LOCKED_BY_OTHER";
        public const string ERROR_ENTITY_VALIDATION = "ERROR.ENTITY.VALIDATION";
        public const string ERROR_GENERIC_COMMON_EXCEPTION = "ERROR.GENERIC.COMMON_EXCEPTION";
        public const string ERROR_GENERIC_PROCESS_FAILED = "ERROR.GENERIC.PROCESS_FAILED";
        public const string ERROR_SYSTEM_CALL_SERVICE = "ERROR.SYSTEM.CALL_SERVICE";
        public const string ERROR_SYSTEM_NOT_SUPPORTED = "ERROR.SYSTEM.NOT_SUPPORTED";
        public const string ERROR_SYSTEM_SECURITY_EXCEPTION = "ERROR.SYSTEM.SECURITY_EXCEPTION";
        public const string ERROR_ENTITY_PARSE = "ERROR.ENTITY.PARSE";

        public class DetailCode
        {
            public const string ERROR_VALIDATION = "ERROR.VALIDATION.FAILED";
            public const string ERROR_VALIDATION_REQUIRED = "ERROR.ENTITY.VALIDATION.FIELD_REQUIRED";
            public const string ERROR_VALIDATION_NOT_FOUND = "ERROR.ENTITY.VALIDATION.FIELD_NOT_FOUND";
            public const string ERROR_VALIDATION_SOME_ITEMS_DELETED = "ERROR.ENTITY.VALIDATION.SOME_ITEMS_DELETED";
            public const string ERROR_VALIDATION_EXISTED = "ERROR.ENTITY.VALIDATION.VALUE_EXISTED";
            public const string ERROR_VALIDATION_INVALID = "ERROR.ENTITY.VALIDATION.FIELD_INVALID";
            public const string ERROR_VALIDATION_DUPLICATED = "ERROR.ENTITY.VALIDATION.FIELD_DUPLICATED";
            public const string ERROR_VALIDATION_MAX_LENGTH = "ERROR.ENTITY.VALIDATION.FIELD_MAX_LENGTH";
            public const string ERROR_VALIDATION_NOT_ACTIVE = "ERROR.ENTITY.VALIDATION.FIELD_NOT_ACTIVE";
            public const string ERROR_VALIDATION_OUT_OF_RANGE = "ERROR.ENTITY.VALIDATION.FIELD_OUT_OF_RANGE";
            public const string ERROR_VALIDATION_GT = "ERROR.ENTITY.VALIDATION.FIELD_GT";
            public const string ERROR_VALIDATION_GTE = "ERROR.ENTITY.VALIDATION.FIELD_GTE";
            public const string ERROR_VALIDATION_LT = "ERROR.ENTITY.VALIDATION.FIELD_LT";
            public const string ERROR_VALIDATION_LTE = "ERROR.ENTITY.VALIDATION.FIELD_LTE";
            public const string ERROR_VALIDATION_UNEXPECTED_FORMAT = "ERROR.ENTITY.VALIDATION.FIELD_UNEXPECTED_FORMAT";

            public const string ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED = "ERROR.ENTITY.NOT_FOUND.SOME_ITEMS_DELETED";
            public const string ERROR_SYSTEM_NOT_SUPPORTED_VERSION_MISMATCH = "ERROR.SYSTEM.NOT_SUPPORTED.VERSION_MISMATCH";
            public const string ERROR_GENERIC_PROCESS_FAILED_DATABASE = "ERROR.GENERIC.PROCESS_FAILED.DATABASE";
            public const string ERROR_ENTITY_INVALID_PUBLISHING = "ERROR.ENTITY.INVALID.PUBLISHING";
            public const string ERROR_ENTITY_INVALID_NON_EDITABLE = "ERROR.ENTITY.INVALID.NON_EDITABLE";
            public const string ERROR_ENTITY_INVALID_PUBLISHING_NON_DELETEABLE = "ERROR.ENTITY.INVALID.NON_DELETEABLE";
            public const string ERROR_ENTITY_INVALID_PUBLISHING_NON_DELETEABLE_HAS_TENANT_ADMIN = "ERROR.ENTITY.INVALID.NON_DELETEABLE.HAS_TENANT_ADMIN";
            public const string ERROR_ENTITY_INVALID_NOT_BINDING = "ERROR.ENTITY.INVALID.NOT_BINDING";
            public const string ERROR_ENTITY_INVALID_USER_IS_TENANT_ADMIN = "ERROR.ENTITY.INVALID.USER_IS_TENANT_ADMIN";
            public const string ERROR_ENTITY_DONOT_HAVE_DELETE_PERMISION_SOME_ITEMS = "ERROR.ENTITY.VALIDATION.DONOT_HAVE_DELETE_PERMISION_SOME_ITEMS"; //You do not have permission to delete some selected item(s)
        }
    }
}