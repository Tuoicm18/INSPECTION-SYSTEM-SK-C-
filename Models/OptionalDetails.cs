using System;

namespace PluginICAOClientSDK.Models {
    public class OptionalDetails {
        public string personalNumber { get; set; }
        public string fullName { get; set; }
        public string birthDate { get; set; }
        public string gender { get; set; }
        public string nationality { get; set; }
        public string ethnic { get; set; }
        public string religion { get; set; }
        public string placeOfOrigin { get; set; }
        public string placeOfResidence { get; set; }
        public string personalIdentification { get; set; }
        public string issuanceDate { get; set; }
        public string expiryDate { get; set; }
        public string idDocument { get; set; }
        //Update 2022.09.20 Replace fullNameOfFather & fullNameOfMother with fullNameOfParents
        public string fullNameOfParents { get; set; }
        //public string fullNameOfFather { get; set; }
        //public string fullNameOfMother { get; set; }
        public string fullNameOfSpouse { get; set; }
    }
}
