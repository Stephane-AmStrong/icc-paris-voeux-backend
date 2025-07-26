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
        public const string AtLeastOnePropertyRequired = "At least one of the {0} properties must be set: {1}";
    }

    public static class Entities
    {
        public const string Wish = "Wish";
    }
}
