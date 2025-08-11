using System.Text.RegularExpressions;
using System.Text;
using Azure.AI.OpenAI;
using Azure;
using Newtonsoft.Json;
using System.Web;
using System.Text.Encodings.Web;
using Azure.AI.ContentSafety;
using Azure.Core;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using CR_CoreBot_DataAccess.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using CR_CoreBot_DTO;
using CR_CoreBot_DataAccess;


namespace CR_CoreBot.Models
{
    public class AzureOpenAIChat
    {

        private readonly IConfiguration _configuration;


        //Security Purpose
        private static string contentSafetyEndpoint;
        private static string contentSafetyKey;
        //End
        //CyberSecurity Purpose
        private static string blocklistName;
        //End
        public AzureOpenAIChat(IConfiguration configuration)
        {
            _configuration = configuration;

            //Security Purpose
            contentSafetyEndpoint = _configuration["Credential:ContentSafetyEndpoint"];
            contentSafetyKey = _configuration["Credential:ContentSafetyApiKey"];

            //End
            blocklistName = configuration["Credential:blocklistName"];
        }





        #region Responsible AI Assessment Created by Cybersecurity Team

        public async Task<bool> BlockListAddItems(string blocklistItemText)
        {
            bool ItemsAdded = false;
            bool NewBlockListRequired = true;
            try
            {
                BlocklistClient blocklistClient = new BlocklistClient(new Uri(contentSafetyEndpoint), new AzureKeyCredential(contentSafetyKey));

                var blocklistDescription = "All words in this list will be blocked.";

                var data = new
                {
                    description = blocklistDescription,
                };

                var blocklists = blocklistClient.GetTextBlocklistsAsync();
                await foreach (var blocklist in blocklists)
                {
                    if (blocklist.Name == blocklistName)
                    {
                        NewBlockListRequired = false;
                        break;
                    }
                }
                if (NewBlockListRequired)
                {
                    var createResponse = await blocklistClient.CreateOrUpdateTextBlocklistAsync(blocklistName, RequestContent.Create(data));
                }


                var blocklistItems = new TextBlocklistItem[] { new TextBlocklistItem(blocklistItemText) };
                var addedBlocklistItems = blocklistClient.AddOrUpdateBlocklistItems(blocklistName, new AddOrUpdateTextBlocklistItemsOptions(blocklistItems));

                if (addedBlocklistItems != null && addedBlocklistItems.Value != null)
                {
                    ItemsAdded = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return ItemsAdded;
        }
        public async Task<bool> BlockListDeleteItems(string removeBlocklistItemId)
        {
            BlocklistClient blocklistClient = new BlocklistClient(new Uri(contentSafetyEndpoint), new AzureKeyCredential(contentSafetyKey));

            var removeBlocklistItemIds = new List<string> { removeBlocklistItemId };
            var removeResult = await blocklistClient.RemoveBlocklistItemsAsync(blocklistName, new RemoveTextBlocklistItemsOptions(removeBlocklistItemIds));

            if (removeResult != null && removeResult.Status == 204)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<BlockListItemDetail>> BlockListGetAllItems()
        {
            List<BlockListItemDetail> AllItems = new List<BlockListItemDetail>();
            try
            {
                BlocklistClient blocklistClient = new BlocklistClient(new Uri(contentSafetyEndpoint), new AzureKeyCredential(contentSafetyKey));

                var getBlocklist = blocklistClient.GetTextBlocklist(blocklistName);
                if (getBlocklist != null && getBlocklist.Value != null)
                {
                    var allBlocklistitems = blocklistClient.GetTextBlocklistItemsAsync(blocklistName);
                    await foreach (var blocklistItem in allBlocklistitems)
                    {
                        BlockListItemDetail item = new BlockListItemDetail();
                        item.Text = blocklistItem.Text;
                        item.Description = blocklistItem.Description;
                        item.BlocklistItemId = blocklistItem.BlocklistItemId;
                        AllItems.Add(item);
                        //Console.WriteLine("BlocklistItemId: {0}, Text: {1}, Description: {2}", blocklistItem.BlocklistItemId, blocklistItem.Text, blocklistItem.Description);
                    }
                }
            }
            catch (Azure.RequestFailedException ex)
            {

            }
            return AllItems;
        }


        #endregion
    }

    public class AzureDTO
    {
        public List<string> source_document { get; set; }
        public string qa_result { get; set; }
        public string qa_result1 { get; set; }
        public string total_tokens { get; set; }
        public string prompt_tokens { get; set; }
        public string Completion_tokens { get; set; }
        public decimal total_cost { get; set; }
        public string suggestions { get; set; }
    }

    public class CitationRoot
    {
        public Citation[] citations { get; set; }
        public string intent { get; set; }
    }

    public class Citation
    {
        public string content { get; set; }
        public object id { get; set; }
        public string title { get; set; }
        public object filepath { get; set; }
        public object url { get; set; }
        public Metadata metadata { get; set; }
        public string chunk_id { get; set; }
    }

    public class Metadata
    {
        public string chunking { get; set; }
    }
    public class ChunkRootobject
    {
        public string odatacontext { get; set; }
        public object[] searchanswers { get; set; }
        public ChunkValue[] value { get; set; }
    }

    public class ChunkValue
    {
        public float searchscore { get; set; }
        public ChunkSearchCaptions[] searchcaptions { get; set; }
        public float searchrerankerScore { get; set; }
        public string chunk { get; set; }
    }

    public class ChunkSearchCaptions
    {
        public string text { get; set; }
        public string highlights { get; set; }
    }

    public class BlockListItemDetail
    {
        public string BlocklistItemId { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }
}
