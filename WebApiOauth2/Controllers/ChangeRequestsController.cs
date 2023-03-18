using Microsoft.Owin.Security.OAuth;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using WebApiOauth2.App_Start;
using WebApiOauth2.Models;

namespace WebApiOauth2.Controllers
{
    public class ChangeRequestsController : ApiController
    {
        public MongoDBContext dbcontext = new MongoDBContext();
        public IMongoCollection<ChangeRequestsModel> changeRequestsCollection;
        public IMongoCollection<LoginOAuthModel> loginCollection;

        [Authorize(Roles = "ChangeRequests")]
        [Route("api/v1/ChangeRequests/GetAll")]
        public IHttpActionResult GetAll()
        {
            try
            {
                changeRequestsCollection = dbcontext.database.GetCollection<ChangeRequestsModel>("Change_Requests");

                return Ok(changeRequestsCollection.AsQueryable().ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "ChangeRequests")]
        [Route("api/v1/ChangeRequests/GetRecord")]
        public IHttpActionResult GetRecord([FromUri] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Request ID cannot be empty.");
            }
            else
            {
                try
                {
                    changeRequestsCollection = dbcontext.database.GetCollection<ChangeRequestsModel>("Change_Requests");

                    var result = changeRequestsCollection.AsQueryable().Where(w => w.CR_ID.Equals(id)).FirstOrDefault();

                    if (result == null)
                    {
                        return null;
                    }
                    else
                    {
                        return Ok(result.CR_ID);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }                
            }
        }

        public string GetRequestType(string typecode)
        {
            string type = string.Empty;

            switch (typecode.ToUpper())
            {
                case "HCPAN":
                    type = "Add New HCP";
                    break;
                case "HCPMATT":
                    type = "Modify HCP Attributes";
                    break;
                case "HCPMPA":
                    type = "Modify Primary Address";
                    break;
                case "HCPMAD":
                    type = "Modify HCP Address";
                    break;
                case "HCPNA":
                    type = "Add New Address";
                    break;
                case "HCPMS":
                    type = "Modify HCP Status";
                    break;
                case "HCOAN":
                    type = "Add New HCO";
                    break;
                case "HCOMATT":
                    type = "Modify HCO Attributes";
                    break;
                case "HCOMAD":
                    type = "Modify HCO Address";
                    break;
                case "AFFAHA":
                    type = "Add HCP Affiliation";
                    break;
                case "AFFRHA":
                    type = "Remove HCP Affiliation";
                    break;
                case "AFFMHA":
                    type = "Modify HCP Affiliation";
                    break;
            }

            return type;
        }

        public void InsertRecord(ChangeRequestsModel changerequest) =>
            changeRequestsCollection.InsertOne(changerequest);

        [Authorize(Roles = "ChangeRequests")]
        [Route("api/v1/ChangeRequests/Create")]
        [HttpPost]
        public IHttpActionResult Create([FromBody] AuthModel auth, [FromUri] string req_type, [FromUri] string source, [FromUri] ChangeRequestsModel change_request, int length = 9)
        {
            ChangeRequestsModel cr = new ChangeRequestsModel();

            try
            {
                loginCollection = dbcontext.database.GetCollection<LoginOAuthModel>("LoginOAuth");
                var user = loginCollection.AsQueryable().Where(w => w.username.Equals(auth.username) && w.password.Equals(auth.password)).ToList();

                // Verification
                if (user == null || user.Count() <= 0)
                {
                    return BadRequest("Either username or password is incorrect");
                }

                var rndDigits = new StringBuilder().Insert(0, "0123456789", length).ToString().ToCharArray();
                var dtDigit = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                var aDigit = string.Join("", rndDigits.OrderBy(o => Guid.NewGuid()).Take(length));
                string crid = "";

                string id = string.Concat("API" + dtDigit, aDigit);

                try
                {
                    crid = GetRecord(id).ToString();
                }
                catch (Exception e)
                {
                    crid = "";
                }

                if (string.IsNullOrEmpty(crid))
                {
                    string tCodeType = "";

                    if (req_type.Substring(0, 3) == "HCP")
                    {
                        tCodeType = "HCP";
                    }
                    else if (req_type.Substring(0, 3) == "HCO")
                    {
                        tCodeType = "HCO";
                    }
                    else if (req_type.Substring(0, 3) == "AFF")
                    {
                        tCodeType = "Affiliation";
                    }

                    var now = DateTime.Now;
                    var today_date = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

                    cr.CR_ID = id;
                    cr.CR_CREATION_DATE = Convert.ToDateTime(today_date.ToString("o"));
                    cr.CR_TYPE = tCodeType;
                    cr.CR_PROCESSED_DATE = null;
                    cr.REQ_TYPE = GetRequestType(req_type);
                    cr.CR_STATUS = "Open";
                    cr.CR_MDM_COMMENT = null;
                    cr.SRC_NAME = source;
                    cr.SRC_CR_ID = change_request.SRC_CR_ID;
                    cr.SRC_CR_USER = change_request.SRC_CR_USER;
                    cr.SRC_CR_USER_TERR_ID = change_request.SRC_CR_USER_TERR_ID;
                    cr.SRC_CR_USER_TERR_NAME = change_request.SRC_CR_USER_TERR_NAME;
                    cr.SRC_CR_USER_COMMENT = change_request.SRC_CR_USER_COMMENT;
                    cr.SRC_HCP_ID = change_request.SRC_HCP_ID;
                    cr.SRC_HCO_ID = change_request.SRC_HCO_ID;
                    cr.SRC_AFF_ID = change_request.SRC_AFF_ID;
                    cr.CR_STEWARDSHIP_ID = null;
                    cr.CR_EXCEPTION_STATUS = null;
                    cr.CR_EXCEPTION_CREATED = null;
                    cr.CR_EXCEPTION_DESC = null;
                    cr.CR_EXCEPTION_ACTION = null;
                    cr.CR_EXCEPTION_PROCESSED = null;

                    if (tCodeType.Equals("HCP"))
                    {
                        if (!string.IsNullOrEmpty(change_request.HCP_MDM_ID))
                        {
                            cr.HCP_MDM_ID = change_request.HCP_MDM_ID;
                        }
                        else
                        {
                            return BadRequest("HCP ID is missing.");
                        }

                        // Add New HCP || Modify HCP Attribute
                        if (req_type.Equals("HCPAN") || req_type.Equals("HCPMATT"))
                        {
                            if (string.IsNullOrEmpty(change_request.HCP_FIRST_NAME))
                            {
                                return BadRequest("First Name is required.");
                            }
                            else
                            {
                                cr.HCP_FIRST_NAME = change_request.HCP_FIRST_NAME;
                            }

                            if (string.IsNullOrEmpty(change_request.HCP_LAST_NAME))
                            {
                                return BadRequest("Last Name is required.");
                            }
                            else
                            {
                                cr.HCP_LAST_NAME = change_request.HCP_LAST_NAME;
                            }
                        }

                        // Add New Address || Modify Primary Address || Modify HCP Address || 
                        if (req_type.Equals("HCPNA") || req_type.Equals("HCPMPA") || req_type.Equals("HCPMAD"))
                        {
                            cr.HCP_FIRST_NAME = change_request.HCP_FIRST_NAME;
                            cr.HCP_LAST_NAME = change_request.HCP_LAST_NAME;

                            if (string.IsNullOrEmpty(change_request.HCP_ADDR_1))
                            {
                                return BadRequest("Address is required.");
                            }
                            else
                            {
                                cr.HCP_ADDR_1 = change_request.HCP_ADDR_1;
                            }

                            if (string.IsNullOrEmpty(change_request.HCP_CITY))
                            {
                                return BadRequest("City is required.");
                            }
                            else
                            {
                                cr.HCP_CITY = change_request.HCP_CITY;
                            }

                            if (string.IsNullOrEmpty(change_request.HCP_STATE))
                            {
                                return BadRequest("State is required.");
                            }
                            else
                            {
                                cr.HCP_STATE = change_request.HCP_STATE;
                            }

                            if (string.IsNullOrEmpty(change_request.HCP_ZIP5))
                            {
                                return BadRequest("Zip Code is required.");
                            }
                            else
                            {
                                cr.HCP_ZIP5 = change_request.HCP_ZIP5;
                            }
                        }

                        cr.HCP_MIDDLE_NAME = change_request.HCP_MIDDLE_NAME;
                        cr.HCP_ADDR_2 = change_request.HCP_ADDR_2;
                        cr.HCP_ZIP4 = change_request.HCP_ZIP4;
                        cr.HCP_ADDR_LAT = change_request.HCP_ADDR_LAT;
                        cr.HCP_ADDR_LON = change_request.HCP_ADDR_LON;
                        cr.HCP_ADDRESS_ID = change_request.HCP_ADDRESS_ID;
                        cr.HCP_CASS_VAL = null;
                        cr.HCP_PRY_SPECIALTY = change_request.HCP_PRY_SPECIALTY;
                        cr.HCP_PRY_SPE_GRP = change_request.HCP_PRY_SPE_GRP;
                        cr.HCP_SEC_SPECIALTY = change_request.HCP_SEC_SPECIALTY;
                        cr.HCP_CREDENTIALS = change_request.HCP_CREDENTIALS;
                        cr.HCP_SCHOOL_NAME = change_request.HCP_SCHOOL_NAME;
                        cr.HCP_GRDTN_YEAR = change_request.HCP_GRDTN_YEAR;
                        cr.HCP_YOB = change_request.HCP_YOB;
                        cr.HCP_YOD = change_request.HCP_YOD;
                        cr.HCP_PDRP_OPT_OUT = change_request.HCP_PDRP_OPT_OUT;
                        cr.HCP_PDRP_OPT_DATE = change_request.HCP_PDRP_OPT_DATE;
                        cr.HCP_PDRP_NO_CONTACT = change_request.HCP_PDRP_NO_CONTACT;
                        cr.HCP_NPI_ID = change_request.HCP_NPI_ID;
                        cr.HCP_SHS_ID = change_request.HCP_SHS_ID;
                        cr.HCP_CRM_ID = change_request.HCP_CRM_ID;
                        cr.HCP_AMA_ID = change_request.HCP_AMA_ID; // ME_ID
                        cr.HCP_DEA_ID = change_request.HCP_DEA_ID;
                        cr.HCP_AOA_ID = change_request.HCP_AOA_ID;
                        cr.HCP_ADA_ID = change_request.HCP_ADA_ID;
                        cr.HCP_AAPA_ID = change_request.HCP_AAPA_ID;
                        cr.HCP_ACNM_ID = change_request.HCP_ACNM_ID;
                        cr.HCP_MSCHST_ID = change_request.HCP_MSCHST_ID;
                        cr.HCP_AOPA_ID = change_request.HCP_AOPA_ID;
                        cr.HCP_MEDICAID_ID = change_request.HCP_MEDICAID_ID;
                        cr.HCP_OPENDATA_ID = change_request.HCP_OPENDATA_ID;
                        cr.HCP_ONEKEY_ID = change_request.HCP_ONEKEY_ID;
                        cr.HCP_UPIN_ID = change_request.HCP_UPIN_ID;
                        cr.HCP_FED_TAX_ID = change_request.HCP_FED_TAX_ID;
                        cr.HCP_URL = change_request.HCP_URL;
                        cr.HCP_SLN = change_request.HCP_SLN;
                        cr.HCP_APMA_ID = change_request.HCP_APMA_ID;
                        cr.HCP_TARGET = change_request.HCP_TARGET;
                        cr.HCP_STATUS = change_request.HCP_STATUS;
                        cr.HCP_DECILE = change_request.HCP_DECILE;
                        cr.HCP_FAX = change_request.HCP_FAX;
                        cr.HCP_EMAIL = change_request.HCP_EMAIL;
                        cr.HCP_PHONE = change_request.HCP_PHONE;
                        cr.HCP_TIER = change_request.HCP_TIER;
                    }

                    if (tCodeType.Equals("HCO"))
                    {
                        if (!string.IsNullOrEmpty(change_request.HCO_MDM_ID))
                        {
                            cr.HCO_MDM_ID = change_request.HCO_MDM_ID;
                        }
                        else
                        {
                            return BadRequest("HCO ID is missing.");
                        }

                        // Add New HCO || Modify HCO Attributes
                        if (req_type.Equals("HCOAN") || req_type.Equals("HCOMATT"))
                        {
                            if (string.IsNullOrEmpty(change_request.HCO_NAME))
                            {
                                return BadRequest("HCO Name is required.");
                            }
                            else
                            {
                                cr.HCO_NAME = change_request.HCO_NAME;
                            }
                        }

                        // Add New HCO || Modify HCO Address
                        if (req_type.Equals("HCOAN") || req_type.Equals("HCOMAD"))
                        {
                            cr.HCO_NAME = change_request.HCO_NAME;

                            if (string.IsNullOrEmpty(change_request.HCO_ADDR_1))
                            {
                                return BadRequest("Address is required.");
                            }
                            else
                            {
                                cr.HCO_ADDR_1 = change_request.HCO_ADDR_1;
                            }

                            if (string.IsNullOrEmpty(change_request.HCO_CITY))
                            {
                                return BadRequest("City is required.");
                            }
                            else
                            {
                                cr.HCO_CITY = change_request.HCO_CITY;
                            }

                            if (string.IsNullOrEmpty(change_request.HCO_STATE))
                            {
                                return BadRequest("State is required.");
                            }
                            else
                            {
                                cr.HCO_STATE = change_request.HCO_STATE;
                            }

                            if (string.IsNullOrEmpty(change_request.HCO_ZIP5))
                            {
                                return BadRequest("Zip Code is required.");
                            }
                            else
                            {
                                cr.HCO_ZIP5 = change_request.HCO_ZIP5;
                            }
                        }

                        cr.HCO_ADDR_2 = change_request.HCO_ADDR_2;
                        cr.HCO_ZIP4 = change_request.HCO_ZIP4;
                        cr.HCO_ADDR_LAT = change_request.HCO_ADDR_LAT;
                        cr.HCO_ADDR_LON = change_request.HCO_ADDR_LON;
                        cr.HCO_ADDRESS_ID = change_request.HCO_ADDRESS_ID;
                        cr.HCO_CASS_VAL = change_request.HCO_CASS_VAL;
                        cr.HCO_PRY_SPECIALTY = change_request.HCO_PRY_SPECIALTY;
                        cr.HCO_PRY_SPE_GRP = change_request.HCO_PRY_SPE_GRP;
                        cr.HCO_SEC_SPECIALTY = change_request.HCO_SEC_SPECIALTY;
                        cr.HCO_SEC_SPE_GRP = change_request.HCO_SEC_SPE_GRP;
                        cr.HCO_COT = change_request.HCO_COT;
                        cr.HCO_COT_GRP = change_request.HCO_COT_GRP;
                        cr.HCO_NPI_ID = change_request.HCO_NPI_ID;
                        cr.HCO_SHS_ID = change_request.HCO_SHS_ID;
                        cr.HCO_CRM_ID = change_request.HCO_CRM_ID;
                        cr.HCO_DEA_ID = change_request.HCO_DEA_ID;
                        cr.HCO_HIN_ID = change_request.HCO_HIN_ID;
                        cr.HCO_DUNS_ID = change_request.HCO_DUNS_ID;
                        cr.HCO_POS_ID = change_request.HCO_POS_ID;
                        cr.HCO_FED_TAX_ID = change_request.HCO_FED_TAX_ID;
                        cr.HCO_GLN_ID = change_request.HCO_GLN_ID;
                        cr.HCO_OPENDATA_ID = change_request.HCO_OPENDATA_ID;
                        cr.HCO_ONEKEY_ID = change_request.HCO_ONEKEY_ID;
                        cr.HCO_URL = change_request.HCO_URL;
                        cr.HCO_IDN = change_request.HCO_IDN;
                        cr.HCO_340B = change_request.HCO_340B;
                        cr.HCO_STATUS = change_request.HCO_STATUS;
                        cr.HCO_TARGET = change_request.HCO_TARGET;
                        cr.HCO_DECILE = change_request.HCO_DECILE;
                        cr.HCO_FACILITY_TYPE = change_request.HCO_FACILITY_TYPE;
                        cr.HCO_FAX = change_request.HCO_FAX;
                        cr.HCO_EMAIL = change_request.HCO_EMAIL;
                        cr.HCO_PHONE = change_request.HCO_PHONE;
                        cr.HCO_TIER = change_request.HCO_TIER;
                    }

                    if (tCodeType.Equals("Affiliation"))
                    {
                        cr.AFF_ID = change_request.AFF_ID;
                        cr.AFF_CHILD_TYPE = change_request.AFF_CHILD_TYPE;
                        cr.AFF_CHILD_OTH_ID_TYPE = change_request.AFF_CHILD_OTH_ID_TYPE;
                        cr.AFF_CHILD_OTH_ID_VAL = change_request.AFF_CHILD_OTH_ID_VAL;
                        cr.AFF_PARENT_TYPE = change_request.AFF_PARENT_TYPE;
                        cr.AFF_PARENT_OTH_ID_TYPE = change_request.AFF_PARENT_OTH_ID_TYPE;
                        cr.AFF_PARENT_OTH_ID_VAL = change_request.AFF_PARENT_OTH_ID_VAL;
                        cr.AFF_START_DATE = change_request.AFF_START_DATE;
                        cr.AFF_TYPE = change_request.AFF_TYPE;
                        cr.AFF_CHILD_ID = change_request.AFF_CHILD_ID;
                        cr.AFF_SRC_CHILD_ID = change_request.AFF_SRC_CHILD_ID;
                        cr.AFF_PARENT_ID = change_request.AFF_PARENT_ID;
                        cr.AFF_SRC_PARENT_ID = change_request.AFF_SRC_PARENT_ID;
                        cr.AFF_END_DATE = change_request.AFF_END_DATE;
                        cr.AFF_DESCRIPTION = change_request.AFF_DESCRIPTION;
                        cr.AFF_PRIMARY = change_request.AFF_PRIMARY;
                    }
                }
                else
                {
                    Create(auth, req_type, source, change_request, 9);

                    InsertRecord(change_request);

                    return Ok("Record Created " + id);
                }

                InsertRecord(cr);

                return Ok("Record Created " + id);
            }
            catch (WebException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}