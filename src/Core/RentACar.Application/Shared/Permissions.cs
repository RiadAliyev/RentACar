namespace RentACar.Application.Shared;

public static class Permissions
{

    public static class Role
    {
        public const string Create = "Role.Create";
        public const string Update = "Role.Update";
        public const string Delete = "Role.Delete";
        public const string GetAllPermission = "Role.GetAllPermission";

        public static List<string> All = new()
        {
            Create,
            GetAllPermission,
            Update,
            Delete
        };
    }
    public static class Booking
    {
        public const string Create = "Booking.Create";
        public const string Update = "Booking.Update";
        public const string Delete = "Booking.Delete";
        public const string GetById = "Booking.GetById";
        public const string GetAll = "Booking.GetAll";

        public static List<string> All = new()
        {
            Create, 
            Update, 
            Delete,
            GetAll,
            GetById
        };
    }

    public static class Car
    {
        public const string Create = "Car.Create";
        public const string Update = "Car.Update";
        public const string Delete = "Car.Delete";
        public const string GetById = "Car.GetById";
        public const string GetAll = "Car.GetAll";
        public const string GetByFilter = "Car.GetByFilter";

        public static List<string> All = new()
        {
            Create,
            Update,
            Delete,
            GetById,
            GetAll,
            GetByFilter

        };
    }

    public static class Account
    {
        public const string AddRole = "Account.AddRole";
        public const string Create = "Account.Create";


        public static List<string> All = new()
        {
            AddRole,
            Create      
        };
    }

    public static class CarFeature
    {
        public const string Create = "CarFeature.Create";
        public const string Update = "CarFeature.Update";
        public const string Delete = "CarFeature.Delete";       
        public const string GetAll = "CarFeature.GetAll";
        public const string GetById = "CarFeature.GetById";


        public static List<string> All = new()
        {           
            Create,
            Update,
            Delete,
            GetById,
            GetAll
        };
    }

    public static class CarImage
    {
        public const string Create = "CarImage.Create";
        public const string Update = "CarImage.Update";
        public const string Delete = "CarImage.Delete";
        public const string GetAll = "CarImage.GetAll";
        public const string GetById = "CarImage.GetById";


        public static List<string> All = new()
        {
            Create,
            Update,
            Delete,
            GetById,
            GetAll
        };
    }

    public static class Company
    {
        public const string Create = "Company.Create";
        public const string Update = "Company.Update";
        public const string Delete = "Company.Delete";
        public const string GetAll = "Company.GetAll";
        public const string GetById = "Company.GetById";


        public static List<string> All = new()
        {
            Create,
            Update,
            Delete,
            GetById,
            GetAll
        };
    }

    public static class Payment
    {
        public const string Create = "Payment.Create";
        public const string Update = "Payment.Update";
        public const string Delete = "Payment.Delete";
        public const string GetAll = "Payment.GetAll";
        public const string GetById = "Payment.GetById";
        public const string GetByBookingId = "Payment.GetByBookingId";


        public static List<string> All = new()
        {
            Create,
            Update,
            Delete,
            GetById,
            GetAll,
            GetByBookingId
        };
    }

    public static class Reviews
    {
        public const string Create = "Reviews.Create";
        public const string Update = "Reviews.Update";
        public const string Delete = "Reviews.Delete";
        public const string GetById = "Reviews.GetById";
        public const string GetAllByCarId = "Reviews.GetAllByCarId";


        public static List<string> All = new()
        {
            Create,
            Update,
            Delete,
            GetById,
            GetAllByCarId
        };
    }
}
