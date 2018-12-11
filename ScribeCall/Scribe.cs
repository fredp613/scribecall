using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using System.Net;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http;

namespace Scribe
{
    public class Scribe : CodeActivity
    {


        [RequiredArgument]
        [Input("Integration event endpoint URL")]
        public InArgument<string> ScribeEndpoint { get; set; }

        [Input("Scribe User Name (email)")]
        [Default("fpearson@eperformanceinc.com")]
        public InArgument<string> ScribeUserName { get; set; }

        [Input("Scribe Password")]
        [Default("Fredp613$")]
        public InArgument<string> ScribePassword { get; set; }


        [Input("Action (1=Create, 2=Update, 4=Delete)")]
        [Default("1")]
        public InArgument<string> Action { get; set; }


        [Output("Response from Scribe")]
        public OutArgument<string> Response { get; set; }


        protected override void Execute(CodeActivityContext executionContext)
        {

            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            tracingService.Trace("Invoking Post API request");

            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Entity entity = (Entity)context.InputParameters["Target"];


            Guid recordId = context.PrimaryEntityId;


            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


            Entity currentRecord = service.Retrieve("ac_sectioncollection", recordId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));


            //string myJson1 = "{'" + this.ParameterName.Get<string>(executionContext) + "': '" + this.ParameterValue.Get<EntityReference>(executionContext).Id.ToString() + "','"
            //+ this.ParameterName1.Get<string>(executionContext) + "'': '" + this.ParameterValue1.Get<EntityReference>(executionContext).Id.ToString() + "'}";


            //string myJson = "{'accountid': '6319274c-4976-e411-82ba-00155d018d00','ac_sectioncollectionid': '5049ab74-06eb-e211-8a0e-2c768a5d3d8f'}";
            string myJson = "{'accountid': '" 
                + currentRecord.GetAttributeValue<EntityReference>("organizationid").Id.ToString() 
                + "','ac_sectioncollectionid': '"+recordId.ToString()+"', 'action':'"+this.Action.Get<string>(executionContext)+"'}";


            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", '" + this.ScribeUserName.Get<string>(executionContext) + "', '" + this.ScribePassword.Get<string>(executionContext) + "'))));

                var response = client.PostAsync(
                   this.ScribeEndpoint.Get<string>(executionContext),
                        new StringContent(myJson, Encoding.UTF8, "application/json"));

                using (HttpContent content = response.Result.Content)
                {
                    // ... Read the string.
                    Task<string> result = content.ReadAsStringAsync();
                   
                    this.Response.Set(executionContext, result.ToString());

                }


            }

        }

    }

    //string myJson = "{'accountid': '6319274c-4976-e411-82ba-00155d018d00','ac_sectioncollectionid': '5049ab74-06eb-e211-8a0e-2c768a5d3d8f'}";
    //        using (var client = new HttpClient())
    //        {
    //            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",Convert.ToBase64String(
    //        System.Text.ASCIIEncoding.ASCII.GetBytes(
    //            string.Format("{0}:{1}", "fpearson@eperformanceinc.com", "Fredp613$"))));

    //            var response = client.PostAsync(
    //               "https://endpoint.scribesoft.com/v1/orgs/23213/requests/9348?accesstoken=fe88dd0d-f277-443e-8888-f1d126c92343",
    //                    new StringContent(myJson, Encoding.UTF8, "application/json"));

    //            using (HttpContent content = response.Result.Content)
    //            {


    //                Task<string> result = content.ReadAsStringAsync();

    //Console.WriteLine(response.Result.ToString());

    //            }


    //        }

    //public class CreateVendor : CodeActivity
    //{


    //    [RequiredArgument]
    //    [Input("Name")]
    //    [Default("Vendor Name")]
    //    public InArgument<string> Name { get; set; }

    //    [RequiredArgument]
    //    [Input("Code")]
    //    [Default("Vendor Code")]
    //    public InArgument<string> Code { get; set; }

    //    [RequiredArgument]
    //    [Input("Business Number")]
    //    [Default("10000000000")]
    //    public InArgument<string> BusinessNumber { get; set; }

    //    [Output("Response from GCPM API - Vendor Code")]
    //    public OutArgument<string> VendorCode { get; set; }

    //    [Output("Response from GCPM API - Response")]
    //    public OutArgument<string> Response { get; set; }

    //    protected override void Execute(CodeActivityContext executionContext)
    //    {

    //        ITracingService tracingService = executionContext.GetExtension<ITracingService>();

    //        tracingService.Trace("Invoking Post API request");



    //        string myJson = "{'VendorName': '" + this.Name.Get<string>(executionContext) + "','VendorBusinessNumber': '" + this.BusinessNumber.Get<string>(executionContext) + "'}";
    //        using (var client = new HttpClient())
    //        {
    //            var response = client.PostAsync(
    //                "https://gcpm-finapi-app.egcs-sesc.ca/api/vendorcustomers",
    //                    new StringContent(myJson, Encoding.UTF8, "application/json"));

    //            using (HttpContent content = response.Result.Content)
    //            {
    //                // ... Read the string.
    //                Task<string> result = content.ReadAsStringAsync();
    //                var vendor = SerializerWrapper.Deserialize<VendorCustomer>(result.Result);
    //                var response1 = "Status: " + response.Result.StatusCode.ToString() +
    //                   " Payload: " + result.Result.ToString();
    //                this.Response.Set(executionContext, response1);
    //                this.VendorCode.Set(executionContext,vendor.vendorCustomerId.ToString());
    //            }


    //        }

    //    }      

    //}

    //public class CreateCustomer : CodeActivity
    //{


    //    [RequiredArgument]
    //    [Input("Name")]
    //    [Default("Customer Name")]
    //    public InArgument<string> Name { get; set; }

    //    [RequiredArgument]
    //    [Input("Code")]
    //    [Default("Customer Code")]
    //    public InArgument<string> Code { get; set; }

    //    [RequiredArgument]
    //    [Input("Business Number")]
    //    [Default("10000000000")]
    //    public InArgument<string> BusinessNumber { get; set; }

    //    [Output("Response from GCPM API - Customer Code")]
    //    public OutArgument<string> CustomerCode { get; set; }

    //    [Output("Response from GCPM API - Response")]
    //    public OutArgument<string> Response { get; set; }

    //    protected override void Execute(CodeActivityContext executionContext)
    //    {

    //        ITracingService tracingService = executionContext.GetExtension<ITracingService>();

    //        tracingService.Trace("Invoking Post API request");



    //        string myJson = "{'CustomerName': '" + this.Name.Get<string>(executionContext) + "','CustomerBusinessNumber': '" + this.BusinessNumber.Get<string>(executionContext) + "'}";
    //        using (var client = new HttpClient())
    //        {
    //            var response = client.PostAsync(
    //                "https://gcpm-finapi-app.egcs-sesc.ca/api/vendorcustomers",
    //                    new StringContent(myJson, Encoding.UTF8, "application/json"));

    //            using (HttpContent content = response.Result.Content)
    //            {
    //                // ... Read the string.
    //                Task<string> result = content.ReadAsStringAsync();
    //                var customer = SerializerWrapper.Deserialize<VendorCustomer>(result.Result);
    //                var response1 = "Status: " + response.Result.StatusCode.ToString() +
    //                   " Payload: " + result.Result.ToString();
    //                this.Response.Set(executionContext, response1);
    //                this.CustomerCode.Set(executionContext, customer.vendorCustomerId.ToString());
    //            }


    //        }

    //    }

    //}

    //public class CreatePayment : CodeActivity
    //{


    //    [RequiredArgument]
    //    [Input("Amount")]
    //    [Default("Amount")]
    //    public InArgument<string> Amount { get; set; }

    //    [RequiredArgument]
    //    [Input("Fiscal Year")]
    //    [Default("2019")]
    //    public InArgument<string> FiscalYear { get; set; }

    //    [RequiredArgument]
    //    [Input("Section 33 user")]
    //    [Default("jdoe")]
    //    public InArgument<string> Section33User { get; set; }
    //    [Output("Response from GCPM API - Payment ID")]
    //    public OutArgument<string> PaymentId { get; set; }

    //    [Output("Response from GCPM API - Response")]
    //    public OutArgument<string> Response { get; set; }

    //    protected override void Execute(CodeActivityContext executionContext)
    //    {

    //        ITracingService tracingService = executionContext.GetExtension<ITracingService>();

    //        tracingService.Trace("Invoking Post API request");



    //        string myJson = "{'Amount': '" + this.Amount.Get<string>(executionContext) + "','FiscalYear': '" + this.FiscalYear.Get<string>(executionContext) + "','Section33User': '" + this.Section33User.Get<string>(executionContext) + "'}";
    //        using (var client = new HttpClient())
    //        {
    //            var response = client.PostAsync(
    //                "https://gcpm-finapi-app.egcs-sesc.ca/api/payments",
    //                    new StringContent(myJson, Encoding.UTF8, "application/json"));

    //            using (HttpContent content = response.Result.Content)
    //            {
    //                // ... Read the string.
    //                Task<string> result = content.ReadAsStringAsync();
    //                var payment = SerializerWrapper.Deserialize<Payment>(result.Result);
    //                var response1 = "Status: " + response.Result.StatusCode.ToString() +
    //                   " Payload: " + result.Result.ToString();
    //                this.Response.Set(executionContext, response1);
    //                this.PaymentId.Set(executionContext, payment.PaymentId.ToString());
    //            }


    //        }

    //    }

    //}

    [DataContract]
    public class VendorCustomer
    {

        [DataMember]
        public Guid vendorCustomerId { get; set; }
        public string customerAddress { get; set; }
        public string custeomerBusinessNumber { get; set; }
        public string vendorAddress { get; set; }
        public string vendorBusinessNumber { get; set; }
        public string vendorCode { get; set; }
        public string vendorName { get; set; }


    }
    [DataContract]
    public class Payment
    {

        [DataMember]
        public Guid PaymentId { get; set; }

        public string Amount { get; set; }

        public string Section33User { get; set; }

    }

    internal class SerializerWrapper
    {
        public static string Serialize<T>(T srcObject)
        {
            using (MemoryStream SerializeMemoryStream = new MemoryStream())
            {
                //initialize DataContractJsonSerializer object and pass AssessmentRequestStandAloneDTO class type to it
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                //write newly created object(assessmentRequest) into memory stream
                serializer.WriteObject(SerializeMemoryStream, srcObject);
                string jsonString = Encoding.Default.GetString(SerializeMemoryStream.ToArray());
                return jsonString;
            }
        }



        public static T Deserialize<T>(string jsonObject)
        {
            using (MemoryStream DeSerializeMemoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                StreamWriter writer = new StreamWriter(DeSerializeMemoryStream);
                writer.Write(jsonObject);
                writer.Flush();
                DeSerializeMemoryStream.Position = 0;

                T deserializedObject = (T)serializer.ReadObject(DeSerializeMemoryStream);
                return deserializedObject;
            }
        }

    }


}

