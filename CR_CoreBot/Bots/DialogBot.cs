// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.18.1

using CR_CoreBot.Controllers;
using CR_CoreBot_DTO;
using CR_HRPortalAI_DataAcess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CR_CoreBot.Bots
{
    public class CR_CoreBot : ActivityHandler
    {
        private readonly IOptions<ConnectionModel> _appSettingconnection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CR_CoreBot(IOptions<ConnectionModel> appSettingconnection, IHttpContextAccessor httpContextAccessor) {
            _appSettingconnection = appSettingconnection;
            _httpContextAccessor = httpContextAccessor; 
        }

        private TaskCompletionSource<bool> responseReadySource;
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            responseReadySource = new TaskCompletionSource<bool>();
            var UserChat = $"{turnContext.Activity.Text}";
            HomeController homeController = new HomeController(_appSettingconnection, _httpContextAccessor);
            DataNewDTO objModel = new DataNewDTO
            {
                PropmpInput = UserChat,
                Document = "HRModel",
                OutScope = "False",
                HyperInput = ""
            };

            var typingTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await turnContext.SendActivityAsync(Microsoft.Bot.Schema.Activity.CreateTypingActivity(), cancellationToken);
                    await Task.Delay(2000, cancellationToken);
                }
            }, cancellationToken);
            string email = string.Empty;
            string username = string.Empty;
            try
            {
                if (turnContext.Activity.ChannelId == "msteams")
                {
                    var userIdss = turnContext.Activity.From.Id;
                    try
                    {
                        var member = await TeamsInfo.GetMemberAsync(turnContext, userIdss, cancellationToken);
                        var userName = member.Name;
                        var userEmail = member.Email ?? "";
                        email = userEmail.ToString();
                        username = userName.ToString();
                    }
                    catch (ErrorResponseException ex)
                    {

                    }
                }
            }
            catch { }

            DataNewDTO FinalResult = await homeController.fnChatReply(objModel,email,username);

            // Send the final response
            await turnContext.SendActivityAsync(MessageFactory.Text(FinalResult.CompletionResult, UserChat), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await SendSamplePrompts(turnContext, FinalResult, cancellationToken);

        }
        private async Task ShowTypingIndicatorAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            while (!responseReadySource.Task.IsCompleted) // Check if the response is ready
            {
                // Show typing indicator
                var typingActivity = Microsoft.Bot.Schema.Activity.CreateTypingActivity();
                await turnContext.SendActivityAsync(typingActivity, cancellationToken);

                // Wait for a specified interval (e.g., 2 seconds) before sending the next typing activity
                await Task.Delay(2000); // Adjust the delay as necessary
            }
        }

        private async Task SendSamplePrompts(ITurnContext turnContext, DataNewDTO finalResult, CancellationToken cancellationToken)
        {
            var samplePrompts = new List<string>();

            // Add the prompts to the list if they are not null or empty
            if (!string.IsNullOrEmpty(finalResult.newSuggestions1))
            {
                samplePrompts.Add(finalResult.newSuggestions1);
            }
            if (!string.IsNullOrEmpty(finalResult.newSuggestions2))
            {
                samplePrompts.Add(finalResult.newSuggestions2);
            }
            if (!string.IsNullOrEmpty(finalResult.newSuggestions3))
            {
                samplePrompts.Add(finalResult.newSuggestions3);
            }

            // Check if there are any sample prompts to send
            if (samplePrompts.Any())
            {
                var buttons = samplePrompts.Select(prompt => new CardAction(ActionTypes.ImBack, prompt, value: prompt)
                {
                    Title = prompt
                }).ToList();

                var heroCard = new HeroCard
                {
                    Title = "What would you like to do next?",
                    Buttons = buttons
                };

                var reply = MessageFactory.Attachment(heroCard.ToAttachment());
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and Welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
