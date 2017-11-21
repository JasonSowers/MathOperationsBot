using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MathOperations.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            int a;
            int b;
            int[] numbers = new int[2];
            bool success = false;

            if (!activity.Text.Contains("Add") &&
                !activity.Text.Contains("Subtract") &&
                !activity.Text.Contains("Multiply") &&
                !activity.Text.Contains("Divide"))
            {
                try
                {
                    string[] strNumbers = activity.Text.Split(' ');
                    numbers[0] = Convert.ToInt32(strNumbers[0]);
                    numbers[1] = Convert.ToInt32(strNumbers[1]);
                    context.ConversationData.SetValue("numbers", numbers);
                    reply.Text = "please select an operation";
                }
                catch (Exception e)
                {
                    reply.Text = "Try again, Please enter 2 numbers seperated by a space";
                    await context.PostAsync(reply);
                    return;
                }
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                    {
                        new CardAction() {Title = "Add", Type = ActionTypes.PostBack, Value = "Add"},
                        new CardAction() {Title = "Subtract", Type = ActionTypes.PostBack, Value = "Subtract"},
                        new CardAction() {Title = "Multiply", Type = ActionTypes.PostBack, Value = "Multiply"},
                        new CardAction() {Title = "Divide", Type = ActionTypes.PostBack, Value = "Divide"}
                    }
                };
                
            }
            else if (activity.Text.Contains("Add") ||
                     activity.Text.Contains("Subtract") ||
                     activity.Text.Contains("Multiply") ||
                     activity.Text.Contains("Divide"))
            {

                numbers =context.ConversationData.GetValue<int[]>("numbers");
                a = numbers[0];
                b = numbers[1];

                switch (activity.Text)
                {
                    case "Add":
                        reply.Text = Sum(a, b).ToString();
                        success = true;
                        break;
                    case "Subtract":
                        reply.Text = Difference(a, b).ToString();
                        success = true;
                        break;
                    case "Multiply":
                        reply.Text = Product(a, b).ToString();
                        success = true;
                        break;
                    case "Divide":
                        reply.Text = Quotient(a, b).ToString();
                        success = true;
                        break;
                }
            }
            await context.PostAsync(reply);
            if (success)
            {
                await context.PostAsync("To try it again please enter 2 numbers separated by a space");
            }
            context.Wait(MessageReceivedAsync);
        }

        private int Sum(int a, int b)
        {
            return a + b;
        }
        private int Difference(int a, int b)
        {
            return a - b;
        }
        private int Product(int a, int b)
        {
            return a * b;
        }
        private double Quotient(int a, int b)
        {
            return (double)a / (double)b;
        }
        
    }
}