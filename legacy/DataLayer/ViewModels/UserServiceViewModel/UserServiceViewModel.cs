using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.ViewModels.UserServiceViewModel
{
    public class UserServiceViewModel
    {

        public class RegisterUser
        {
            public string PhoneNumber { get; set; }
            public string IntroCode { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        public class SubmitOrder
        {
            public string UserId { get; set; }
            public string GuId { get; set; }
            public string Discount { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
            //1 معمولی
            //2 اکسپرس
            public int? Type { get; set; }
            //1 نقدی
            //2 آنلاین
            public int? PaymentType { get; set; }
            public string UserAddress { get; set; }
            public string UserDescription { get; set; }
        }

        public class ProductInCount
        {
            public long Id { get; set; }
            public int Count { get; set; }
        }

        public class SubmitAdd
        {
            public string UserId { get; set; }
            public int? CategoryId { get; set; }
            public string Title { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string FullAddress { get; set; }
            public List<string> ImagePath { get; set; }
            public string UserDescription { get; set; }
            public string UserShortDescription { get; set; }
            public int? CityId { get; set; }
            public int? RegionId { get; set; }
            public string Email { get; set; }
            public string PhoneNumber1 { get; set; }
            public string PhoneNumber2 { get; set; }
            public string PhoneNumber3 { get; set; }
            public string PhoneNumber4 { get; set; }
            public string Fax { get; set; }
            public string Mobile { get; set; }
            public string WebSite { get; set; }
            public string Owner { get; set; }

        }

        public class QuestionModel
        {
            public int? QuestionId { get; set; }
            public string AnswerBody { get; set; }
        }

        public class EditUser
        {
            public string UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public int? StateId { get; set; }
            public int? CityId { get; set; }
            public byte? Gender { get; set; }
        }

        public class InsertAddress
        {
            //public string Title { get; set; }
            public string Address { get; set; }
            public string plaque { get; set; }
            public int unit { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string Type { get; set; }
            public string UserId { get; set; }
        }

        public class EditAddress
        {
            public int AddressId { get; set; }
            public string Address { get; set; }
            public string plaque { get; set; }
            public int unit { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string Type { get; set; }
        }

        public class InsertComplaint
        {
            public int? OrderId { get; set; }
            public int? ContractorId { get; set; }
            public string UserComplaintId { get; set; }
            public string Description { get; set; }
        }

        public class ScoringViewModel
        {
            public byte? Situation { get; set; }
            public string Description { get; set; }
            public string DefaultDescription { get; set; }
            public string UserId { get; set; }
            public int? ContractorId { get; set; }
            public int? OrderId { get; set; }
        }

        public class AddScoringViewModel
        {
            public byte? Situation { get; set; }
            public string Description { get; set; }
            public string DefaultDescription { get; set; }
            public string UserId { get; set; }
            public int? ContractorId { get; set; }
            public int? AddvertisementId { get; set; }
        }
        public class SearchViewModel
        {
            public string entryText { get; set; }
        }
    }
}
