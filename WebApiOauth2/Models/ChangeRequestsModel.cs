using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace WebApiOauth2.Models
{
    public class ChangeRequestsModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("CR_ID")]
        [JsonProperty("CR_ID")]
        public string CR_ID { get; set; }
        public DateTime? CR_CREATION_DATE { get; set; }
        public string CR_TYPE { get; set; }
        public string CR_PROCESSED_DATE { get; set; }
        public string REQ_TYPE { get; set; }
        public string CR_STATUS { get; set; }
        public string CR_MDM_COMMENT { get; set; }
        public string SRC_NAME { get; set; }
        public string SRC_CR_ID { get; set; }
        public string SRC_CR_USER { get; set; }
        public string SRC_CR_USER_TERR_ID { get; set; }
        public string SRC_CR_USER_TERR_NAME { get; set; }
        public string SRC_CR_USER_COMMENT { get; set; }
        public string SRC_HCP_ID { get; set; }
        public string SRC_HCO_ID { get; set; }
        public string SRC_AFF_ID { get; set; }
        public string CR_STEWARDSHIP_ID { get; set; }
        public string CR_EXCEPTION_STATUS { get; set; }
        public string CR_EXCEPTION_CREATED { get; set; }
        public string CR_EXCEPTION_DESC { get; set; }
        public string CR_EXCEPTION_ACTION { get; set; }
        public string CR_EXCEPTION_PROCESSED { get; set; }
        public string HCP_MDM_ID { get; set; }
        public string HCP_LAST_NAME { get; set; }
        public string HCP_MIDDLE_NAME { get; set; }
        public string HCP_FIRST_NAME { get; set; }
        public string HCP_ADDR_1 { get; set; }
        public string HCP_ADDR_2 { get; set; }
        public string HCP_CITY { get; set; }
        public string HCP_STATE { get; set; }
        public string HCP_ZIP5 { get; set; }
        public string HCP_ZIP4 { get; set; }
        public string HCP_ADDR_LAT { get; set; }
        public string HCP_ADDR_LON { get; set; }
        public string HCP_ADDRESS_ID { get; set; }
        public string HCP_CASS_VAL { get; set; }
        public string HCP_PRY_SPECIALTY { get; set; }
        public string HCP_PRY_SPE_GRP { get; set; }
        public string HCP_SEC_SPECIALTY { get; set; }
        public string HCP_CREDENTIALS { get; set; }
        public string HCP_SCHOOL_NAME { get; set; }
        public string HCP_GRDTN_YEAR { get; set; }
        public string HCP_YOB { get; set; }
        public string HCP_YOD { get; set; }
        public string HCP_PDRP_OPT_OUT { get; set; }
        public string HCP_PDRP_OPT_DATE { get; set; }
        public string HCP_PDRP_NO_CONTACT { get; set; }
        public string HCP_NPI_ID { get; set; }
        public string HCP_SHS_ID { get; set; }
        public string HCP_CRM_ID { get; set; }
        public string HCP_AMA_ID { get; set; }
        public string HCP_DEA_ID { get; set; }
        public string HCP_AOA_ID { get; set; }
        public string HCP_ADA_ID { get; set; }
        public string HCP_AAPA_ID { get; set; }
        public string HCP_ACNM_ID { get; set; }
        public string HCP_MSCHST_ID { get; set; }
        public string HCP_AOPA_ID { get; set; }
        public string HCP_MEDICAID_ID { get; set; }
        public string HCP_OPENDATA_ID { get; set; }
        public string HCP_ONEKEY_ID { get; set; }
        public string HCP_UPIN_ID { get; set; }
        public string HCP_FED_TAX_ID { get; set; }
        public string HCP_URL { get; set; }
        public string HCP_SLN { get; set; }
        public string HCP_APMA_ID { get; set; }
        public string HCP_TARGET { get; set; }
        public string HCP_STATUS { get; set; }
        public string HCP_DECILE { get; set; }
        public string HCP_FAX { get; set; }
        public string HCP_EMAIL { get; set; }
        public string HCP_PHONE { get; set; }
        public string HCP_TIER { get; set; }
        public string HCO_MDM_ID { get; set; }
        public string HCO_NAME { get; set; }
        public string HCO_ADDR_1 { get; set; }
        public string HCO_ADDR_2 { get; set; }
        public string HCO_CITY { get; set; }
        public string HCO_STATE { get; set; }
        public string HCO_ZIP5 { get; set; }
        public string HCO_ZIP4 { get; set; }
        public string HCO_ADDR_LAT { get; set; }
        public string HCO_ADDR_LON { get; set; }
        public string HCO_ADDRESS_ID { get; set; }
        public string HCO_CASS_VAL { get; set; }
        public string HCO_PRY_SPECIALTY { get; set; }
        public string HCO_PRY_SPE_GRP { get; set; }
        public string HCO_SEC_SPECIALTY { get; set; }
        public string HCO_SEC_SPE_GRP { get; set; }
        public string HCO_COT { get; set; }
        public string HCO_COT_GRP { get; set; }
        public string HCO_NPI_ID { get; set; }
        public string HCO_SHS_ID { get; set; }
        public string HCO_CRM_ID { get; set; }
        public string HCO_DEA_ID { get; set; }
        public string HCO_HIN_ID { get; set; }
        public string HCO_DUNS_ID { get; set; }
        public string HCO_POS_ID { get; set; }
        public string HCO_FED_TAX_ID { get; set; }
        public string HCO_GLN_ID { get; set; }
        public string HCO_OPENDATA_ID { get; set; }
        public string HCO_ONEKEY_ID { get; set; }
        public string HCO_URL { get; set; }
        public string HCO_IDN { get; set; }
        public string HCO_340B { get; set; }
        public string HCO_STATUS { get; set; }
        public string HCO_TARGET { get; set; }
        public string HCO_DECILE { get; set; }
        public string HCO_FACILITY_TYPE { get; set; }
        public string HCO_FAX { get; set; }
        public string HCO_EMAIL { get; set; }
        public string HCO_PHONE { get; set; }
        public string HCO_TIER { get; set; }
        public string AFF_ID { get; set; }
        public string AFF_CHILD_TYPE { get; set; }
        public string AFF_CHILD_ID { get; set; }
        public string AFF_SRC_CHILD_ID { get; set; }
        public string AFF_PARENT_TYPE { get; set; }
        public string AFF_PARENT_ID { get; set; }
        public string AFF_SRC_PARENT_ID { get; set; }
        public string AFF_CHILD_OTH_ID_TYPE { get; set; }
        public string AFF_CHILD_OTH_ID_VAL { get; set; }
        public string AFF_PARENT_OTH_ID_TYPE { get; set; }
        public string AFF_PARENT_OTH_ID_VAL { get; set; }
        public string AFF_START_DATE { get; set; }
        public string AFF_END_DATE { get; set; }
        public string AFF_TYPE { get; set; }
        public string AFF_DESCRIPTION { get; set; }
        public string AFF_PRIMARY { get; set; }

        [BsonIgnore]
        public string CR_MDM_NOTE { get; set; }

        [BsonIgnore]
        public string HCO_CRM_ADDRESS_ID { get; set; }

        [BsonIgnore]
        public string HCP_CRM_ADDRESS_ID { get; set; }

        [BsonIgnore]
        public string CR_EXCEPTION_NOTES { get; set; }
    }
}