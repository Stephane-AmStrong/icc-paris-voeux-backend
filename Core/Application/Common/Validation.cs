namespace Application.Common;

public static class Validation
{
    public static class Messages
    {
        public const string EntityAlreadyExists = "{0} with id '{{PropertyValue}}' already exists";
        public const string EntityNotFound = "{0} with id '{{PropertyValue}}' does not exist";
        public const string FieldRequired = "{{PropertyName}} is required";
        public const string FieldAlreadyInUse = "{0} '{{PropertyValue}}' is already used by another {1}";
        public const string RelationshipAlreadyExists = "A {0} between this {1} and {2} already exists";
    }

    public static class Entities
    {
        public const string Wish = "Wish";
        public const string Client = "Client";
        public const string Connection = "Connection";
        public const string Pulse = "Pulse";
        public const string User = "User";
    }
}
