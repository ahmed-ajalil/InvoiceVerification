using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_CoreBot_DTO;
using CR_HRPortalAI_DataAcess.CR_HRPortal;
using CR_HRPortalAI_DataAcess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_Service.Adapter
{
    public class HrAdminService
    {
        private readonly ConnectionModel _appSettingconnection;
        private readonly HrportalAiContext connection;
        private readonly CropenAiContext connectionopenAi;
        public HrAdminService(ConnectionModel appSettingconnection)
        {
            _appSettingconnection = appSettingconnection;
            connection = new HrportalAiContext(_appSettingconnection);
            connectionopenAi = new CropenAiContext();
        }
        public BlobConnectionModel? readBlobConnection()
        {
            try
            {
                BlobConnectionModel? blobConnection = null;
                BlobConnection? products = connectionopenAi.BlobConnections.FirstOrDefault();
                if (products != null)
                {
                    blobConnection = new BlobConnectionModel
                    {
                        Id = products.Id,
                        CustomerId = products.CustomerId,
                        AccountName = products.AccountName,
                        AccountKey = products.AccountKey,
                        BlobContainerName = products.BlobContainerName,
                        BlobName = products.BlobName,
                        EndpointSuffix = products.EndpointSuffix,
                        ConnectionString = products.ConnectionString,
                        DateTime = products.DateTime
                    };
                }
                return blobConnection;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CustomerModelDTO? readCustomerModel()
        {
            try
            {
                CustomerModelDTO? customerModel = null;
                CustomerModel? products = connectionopenAi.CustomerModels.FirstOrDefault();
                if (products != null)
                {
                    customerModel = new CustomerModelDTO
                    {
                        Id = products.Id,
                        CustomerId = products.CustomerId,
                        ModelName = products.ModelName,
                        ModelDisplayName = products.ModelDisplayName
                    };
                }
                return customerModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public CustomerInformation? GetCustomerInformation(string LoginUserName)
        {
            try
            {
                CustomerInformation? customerInformation = connectionopenAi.CustomerInformations.Where(x => x.UserName == LoginUserName).FirstOrDefault();
                return customerInformation;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string fnInsertBlobdata(BlobConnection blobConnection, string LoginUser)
        {
            try
            {
                CustomerInformation? customer = connectionopenAi.CustomerInformations.Where(x => x.UserName == LoginUser).FirstOrDefault();
                if (customer != null)
                {
                    BlobConnection? blobConnection1 = connectionopenAi.BlobConnections.Where(x => x.UserName == LoginUser).FirstOrDefault();
                    if (blobConnection1 != null)
                    {
                        BlobConnection blob = new BlobConnection();
                        blob.Id = blobConnection1.Id;
                        blob.CustomerId = customer.CustomerId;
                        blob.UserName = blobConnection1.UserName;
                        blob.AccountName = blobConnection.AccountName;
                        blob.AccountKey = blobConnection.AccountKey;
                        blob.BlobContainerName = blobConnection.BlobContainerName;
                        blob.BlobName = blobConnection1.BlobName;
                        blob.EndpointSuffix = blobConnection1.EndpointSuffix;
                        blob.ConnectionString = blobConnection.ConnectionString;
                        blob.StorageType = blobConnection.StorageType;
                        blob.DateTime = DateTime.Now;
                        using (var ctx = new CropenAiContext())
                        {
                            //ctx.BlobConnections.Add(blob);
                            ctx.Entry(blob).State = EntityState.Modified;
                            ctx.SaveChanges();
                        }
                        return "true";
                    }
                    else
                    {
                        BlobConnection blob = new BlobConnection();

                        blob.CustomerId = customer.CustomerId;
                        blob.UserName = LoginUser;
                        blob.AccountName = blobConnection.AccountName;
                        blob.AccountKey = blobConnection.AccountKey;
                        blob.BlobContainerName = blobConnection.BlobContainerName;
                        blob.BlobName = blobConnection.BlobName;
                        blob.EndpointSuffix = blobConnection.EndpointSuffix;
                        blob.ConnectionString = blobConnection.ConnectionString;
                        blob.StorageType = blobConnection.StorageType;
                        blob.DateTime = DateTime.Now;
                        connectionopenAi.BlobConnections.Add(blob);
                        connectionopenAi.SaveChanges();
                        return "true";
                    }


                }
                else
                {
                    return "Account Doesn't exist";
                }

            }
            catch (Exception ex)
            {
                return "false";
            }
        }
        public bool fnInsertCustomerModel(CustomerModel customer)
        {
            try
            {
                CustomerModel customerModel = new CustomerModel();
                customerModel.CustomerId = customer.CustomerId;
                customerModel.ModelName = customer.ModelName;
                customerModel.ModelDisplayName = customer.ModelDisplayName;
                connectionopenAi.CustomerModels.Add(customerModel);
                connectionopenAi.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool fnInsertModelFineTuneData(ModelFineTuneDatum modelFineTune)
        {
            try
            {
                ModelFineTuneDatum fineTuneDatum = new ModelFineTuneDatum();
                fineTuneDatum.Prompt = modelFineTune.Prompt;
                fineTuneDatum.Completion = modelFineTune.Completion;
                fineTuneDatum.LabelName = modelFineTune.LabelName;
                fineTuneDatum.Url = modelFineTune.Url;
                fineTuneDatum.ModelName = modelFineTune.ModelName;
                fineTuneDatum.CustomerId = modelFineTune.CustomerId;
                fineTuneDatum.Datetime = DateTime.Now;
                connection.ModelFineTuneData.Add(fineTuneDatum);
                connection.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool fnUpdateModelFineTuneData(int Id, ModelFineTuneDatum modelFineTune)
        {
            try
            {
                ModelFineTuneDatum fineTuneDatumup = new ModelFineTuneDatum();
                fineTuneDatumup = connection.ModelFineTuneData.Where(x => x.Id == Id).FirstOrDefault();



                if (fineTuneDatumup != null)
                {
                    //fineTuneDatum.CustomerId = modelFineTune.CustomerId;
                    fineTuneDatumup.Prompt = modelFineTune.Prompt;
                    fineTuneDatumup.Completion = modelFineTune.Completion;
                    fineTuneDatumup.LabelName = modelFineTune.LabelName;
                    fineTuneDatumup.Url = modelFineTune.Url;
                    fineTuneDatumup.ModelName = modelFineTune.ModelName;
                    fineTuneDatumup.CustomerId = modelFineTune.CustomerId;
                    fineTuneDatumup.Datetime = DateTime.Now;
                    connection.Entry(fineTuneDatumup).State = EntityState.Modified;
                    connection.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool fnUpdateUserData(int Id, UserInformation userInformation)
        {
            try
            {
                UserInformation userInfo = new UserInformation();

                userInfo = connection.UserInformations.Where(x => x.UserId == Id).FirstOrDefault();
                if (userInfo != null)
                {
                    //fineTuneDatum.CustomerId = modelFineTune.CustomerId;
                    userInfo.FirstName = userInformation.FirstName;
                    userInfo.LastName = userInformation.LastName;
                    userInfo.UserName = userInformation.UserName;
                    userInfo.Password = userInformation.Password;
                    userInfo.RoleId = userInformation.RoleId;
                    userInfo.CustomerId = userInformation.CustomerId;
                    userInfo.Streaming = userInformation.Streaming;

                    connection.Entry(userInfo).State = EntityState.Modified;
                    connection.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool fnDeleteBlobFileFromDatabase(string filename)
        {
            bool result = false;
            try
            {
                CR_HRPortalAI_DataAcess.CR_HRPortal.BlobFileDatum blobfile = connection.BlobFileData.Where(x => x.FileName == filename).FirstOrDefault();
                if (blobfile != null)
                {
                    var data = connection.BlobFileData.SingleOrDefault(x => x.FileName == filename);
                    if (data != null)
                    {
                        connection.Database.ExecuteSqlRaw("DELETE FROM BlobFileData WHERE FileName = {0}", filename);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                return result;
            }
        }

        public BlobConnectionModel fnGetBlobDetails(string UserName)
        {

            try
            {
                var user = connectionopenAi.CustomerInformations.SingleOrDefault(u => u.UserName == UserName);

                if (user != null)
                {
                    var userCredentials = new BlobConnectionModel
                    {
                        AccountName = user.AccountName,
                        AccountKey = user.AccountKey,
                        BlobContainerName = user.BlobContainerName,
                    };

                    return userCredentials;
                }
                else
                {
                    // Handle the case where the user with the specified username is not found
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
