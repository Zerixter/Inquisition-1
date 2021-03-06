﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Inquisition.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inquisition.Data;

namespace Inquisition.Modules
{
    public class GaneralModule : ModuleBase<SocketCommandContext>
    {
        [Command("poll", RunMode = RunMode.Async)]
        [Alias("poll:")]
        [Summary("Create a poll")]
        public async Task CreatePollAsync([Remainder] string r = "")
        {
            List<Emoji> reactions = new List<Emoji> { new Emoji("👍🏻"), new Emoji("👎🏻"), new Emoji("🤷🏻") };

            var messages = await Context.Channel.GetMessagesAsync(1).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);

            EmbedBuilder embed = EmbedTemplate.Create(Context.Client.CurrentUser, Context.User);
            embed.WithTitle(r);
            embed.WithFooter($"Asked by: {Context.User}", Context.User.GetAvatarUrl());

            var msg = await ReplyAsync("", false, embed.Build());

            foreach (Emoji e in reactions)
            {
                await msg.AddReactionAsync(e);
            }
        }

        [Command("timezone", RunMode = RunMode.Async)]
        [Summary("Tells you your timezone from the database")]
        public async Task ShowTimezoneAsync(SocketUser user = null)
        {
            User local;
            switch (user)
            {
                case null:
                    local = DatabaseHandler.GetFromDb(Context.User);
                    break;
                default:
                    local = DatabaseHandler.GetFromDb(user);
                    break;
            }

            if (local.TimezoneOffset is null)
            {
                await ReplyAsync(Message.Error.TimezoneNotSet);
                return;
            }

            await ReplyAsync(Message.Info.Timezone(local));
        }

        [Command("joke", RunMode = RunMode.Async)]
        [Alias("joke by")]
        [Summary("Displays a random joke by random user unless user is specified")]
        public async Task ShowJokeAsync(SocketUser user = null)
        {
            List<Joke> Jokes;
            Random rn = new Random();
            User localUser;

            switch (user)
            {
                case null:
                    localUser = DatabaseHandler.GetFromDb(Context.User);
                    Jokes = DatabaseHandler.ListAll(new Joke());
                    break;
                default:
                    localUser = DatabaseHandler.GetFromDb(user);
                    Jokes = DatabaseHandler.ListAll(new Joke(), localUser);
                    break;
            }

            if (Jokes.Count > 0)
            {
                Joke joke = Jokes[rn.Next(Jokes.Count)];
                EmbedBuilder embed = EmbedTemplate.Create(Context.Client.CurrentUser, Context.User);
                embed.WithTitle($"{joke.Id} - {joke.Text}");
                embed.WithFooter($"Submitted by: {joke.User.Username}#{joke.User.Discriminator}", joke.User.AvatarUrl);

                await ReplyAsync($"Here you go:", false, embed.Build());
            }
            else
            {
                await ReplyAsync(Message.Error.NoContent(localUser));
            }
        }

        [Command("jokes", RunMode = RunMode.Async)]
        [Alias("jokes by")]
        [Summary("Shows a list of all jokes from all users unless user is specified")]
        public async Task ListJokesAsync(SocketUser user = null)
        {
            List<Joke> Jokes;
            User localUser;

            switch (user)
            {
                case null:
                    Jokes = DatabaseHandler.ListAll(new Joke());
                    localUser = DatabaseHandler.GetFromDb(Context.User);
                    break;
                default:
                    localUser = DatabaseHandler.GetFromDb(user);
                    Jokes = DatabaseHandler.ListAll(new Joke(), localUser);
                    break;
            }

            if (Jokes.Count > 0)
            {
                EmbedBuilder embed = EmbedTemplate.Create(Context.Client.CurrentUser, Context.User);

                foreach (Joke joke in Jokes)
                {
                    embed.AddField($"{joke.Id} - {joke.Text}", $"Submitted by {joke.User.Username} on {joke.CreatedAt}");
                }

                await ReplyAsync(Message.Info.Generic, false, embed.Build());
            }
            else
            {
                await ReplyAsync(Message.Error.NoContent(localUser));
            }
        }

        [Command("meme", RunMode = RunMode.Async)]
        [Alias("meme by")]
        [Summary("Displays a random meme by random user unless user is specified")]
        public async Task ShowMemeAsync(SocketUser user = null)
        {
            List<Meme> Memes;
            Random rn = new Random();
            User localUser;

            switch (user)
            {
                case null:
                    localUser = DatabaseHandler.GetFromDb(Context.User);
                    Memes = DatabaseHandler.ListAll(new Meme());
                    break;
                default:
                    localUser = DatabaseHandler.GetFromDb(user);
                    Memes = DatabaseHandler.ListAll(new Meme(), localUser);
                    break;
            }

            if (Memes.Count > 0)
            {
                Meme meme = Memes[rn.Next(Memes.Count)];
                EmbedBuilder embed = EmbedTemplate.Create(Context.Client.CurrentUser, Context.User);
                embed.WithFooter($"Submitted by: {meme.User.Username}#{meme.User.Discriminator}", meme.User.AvatarUrl);
                embed.WithImageUrl(meme.Url);
                embed.WithTitle($"{meme.Id} - {meme.Url}");

                await ReplyAsync(Message.Info.Generic, false, embed.Build());
            }
            else
            {
                await ReplyAsync(Message.Error.NoContent(localUser));
            }
        }

        [Command("meme random", RunMode = RunMode.Async)]
        [Alias("random meme")]
        [Summary("Shows a random meme")]
        public async Task ShowRandomMemeAsync()
        {
            Random rn = new Random();
            int limit = 33000;

            string Path(int n) => $"http://images.memes.com/meme/{n}.jpg";

            string meme = Path(rn.Next(limit));

            EmbedBuilder embed = EmbedTemplate.Create(Context.Client.CurrentUser, Context.User);
            embed.WithImageUrl(meme);
            embed.WithTitle(meme);

            await ReplyAsync(Message.Info.Generic, false, embed.Build());
        }

        [Command("memes", RunMode = RunMode.Async)]
        [Alias("memes by")]
        [Summary("Shows a list of all memes from all users unless user is specified")]
        public async Task ListMemesAsync(SocketUser user = null)
        {
            List<Meme> Memes;
            User local;

            switch (user)
            {
                case null:
                    Memes = DatabaseHandler.ListAll(new Meme());
                    local = DatabaseHandler.GetFromDb(Context.User);
                    break;
                default:
                    local = DatabaseHandler.GetFromDb(user);
                    Memes = DatabaseHandler.ListAll(new Meme(), local);
                    break;
            }

            if (Memes.Count > 0)
            {
                EmbedBuilder embed = EmbedTemplate.Create(Context.Client.CurrentUser, Context.User);

                foreach (Meme meme in Memes)
                {
                    embed.AddField($"{meme.Id} - {meme.Url}", $"Submitted by {meme.User.Username} on {meme.CreatedAt}");
                }

                await ReplyAsync(Message.Info.Generic, false, embed.Build());
            }
            else
            {
                await ReplyAsync(Message.Error.NoContent(local));
            }
        }

        [Command("reminders", RunMode = RunMode.Async)]
        [Summary("Displays a list with all of your reminders")]
        public async Task ListRemindersAsync()
        {
            User localUser = DatabaseHandler.GetFromDb(Context.User);
            List<Reminder> Reminders = DatabaseHandler.ListAll(new Reminder(), localUser);

            if (Reminders.Count > 0)
            {
                EmbedBuilder embed = EmbedTemplate.Create(Context.Client.CurrentUser, Context.User);

                foreach (Reminder reminder in Reminders)
                {
                    embed.AddField($"{reminder.Id} - {reminder.Message ?? "No message"}", $"{reminder.DueDate}");
                }

                await ReplyAsync(Message.Info.Generic, false, embed.Build());
            }
            else
            {
                await ReplyAsync(Message.Error.NoContentGeneric);
            }
        }

        [Command("alerts", RunMode = RunMode.Async)]
        [Summary("Displays a list of all of your notifications")]
        public async Task ListAlertsAsync()
        {
            User localUser = DatabaseHandler.GetFromDb(Context.User);
            List<Alert> Alerts = DatabaseHandler.ListAll(new Alert(), localUser);

            if (Alerts.Count == 0)
            {
                await ReplyAsync(Message.Error.NoContentGeneric);
                return;
            }

            EmbedBuilder embed = EmbedTemplate.Create(Context.Client.CurrentUser, Context.User);

            foreach (Alert n in Alerts)
            {
                embed.AddField($"For when {n.TargetUser.Username} joins", $"Created: {n.CreatedAt}");
            }

            await ReplyAsync(Message.Info.Generic, false, embed.Build());
        }
    }

    [Group("add")]
    public class AddGeneralModule : ModuleBase<SocketCommandContext>
    {
        [Command("joke", RunMode = RunMode.Async)]
        [Summary("Adds a new joke")]
        public async Task AddJokeAsync([Remainder] string jokeText)
        {
            User localUser = DatabaseHandler.GetFromDb(Context.User);

            if (jokeText is null)
            {
                await ReplyAsync(Message.Error.IncorrectStructure(new Joke()));
                return;
            }

            Joke joke = new Joke
            {
                Text = jokeText,
                User = localUser
            };

            switch (DatabaseHandler.AddToDb(joke))
            {
                case DatabaseHandler.Result.Successful:
                    await ReplyAsync(Message.Info.SuccessfullyAdded(joke));
                    break;
                default:
                    await ReplyAsync(Message.Error.Generic);
                    break;
            }
        }

        [Command("meme", RunMode = RunMode.Async)]
        [Summary("Adds a new meme")]
        public async Task AddMemeAsync([Remainder] string url)
        {
            User localUser = DatabaseHandler.GetFromDb(Context.User);

            if (url is null)
            {
                await ReplyAsync(Message.Error.IncorrectStructure(new Meme()));
                return;
            }

            Meme meme = new Meme
            {
                Url = url,
                User = localUser
            };

            switch (DatabaseHandler.AddToDb(meme))
            {
                case DatabaseHandler.Result.Successful:
                    await ReplyAsync(Message.Info.SuccessfullyAdded(meme));
                    break;
                default:
                    await ReplyAsync(Message.Error.Generic);
                    break;
            }
        }

        [Command("reminder", RunMode = RunMode.Async)]
        [Summary("Add a new reminder")]
        public async Task AddReminderAsync(string dueDate, [Remainder] string remainder = "")
        {
            User localUser = DatabaseHandler.GetFromDb(Context.User);

            if (localUser.TimezoneOffset is null)
            {
                await ReplyAsync(Message.Error.TimezoneNotSet);
                return;
            }

            DateTimeOffset dueDateUtc = new DateTimeOffset(DateTime.Parse(dueDate),
                                                           new TimeSpan((int)localUser.TimezoneOffset, 0, 0));

            Reminder reminder = new Reminder
            {
                CreateDate = DateTimeOffset.UtcNow,
                DueDate = dueDateUtc,
                Message = remainder,
                User = localUser
            };

            switch (DatabaseHandler.AddToDb(reminder))
            {
                case DatabaseHandler.Result.Successful:
                    await ReplyAsync(Message.Info.SuccessfullyAdded(reminder));
                    break;
                default:
                    await ReplyAsync(Message.Error.Generic);
                    break;
            }
        }

        [Command("alert", RunMode = RunMode.Async)]
        [Summary("Add a new alert, must specify a target user")]
        public async Task AddAlertAsync(SocketUser user = null)
        {
            User localUserAuthor = DatabaseHandler.GetFromDb(Context.User);

            if (localUserAuthor.TimezoneOffset is null)
            {
                await ReplyAsync(Message.Error.TimezoneNotSet);
                return;
            }

            if (user is null)
            {
                await ReplyAsync(Message.Error.IncorrectStructure(new Alert()));
                return;
            }

            User localUserTarget = DatabaseHandler.GetFromDb(user);

            DateTimeOffset creationDate =
                new DateTimeOffset(DateTime.Now, new TimeSpan((int)localUserAuthor.TimezoneOffset, 0, 0));

            Alert n = new Alert
            {
                User = localUserAuthor,
                TargetUser = localUserTarget,
                CreatedAt = creationDate
            };

            switch (DatabaseHandler.AddToDb(n))
            {
                case DatabaseHandler.Result.Successful:
                    await ReplyAsync(Message.Info.SuccessfullyAdded(n));
                    break;
                default:
                    await ReplyAsync(Message.Error.Generic);
                    break;
            }
        }
    }

    [Group("remove")]
    public class RemoveGeneralModule : ModuleBase<SocketCommandContext>
    {
        [Command("joke")]
        [Summary("Delete a joke")]
        public async Task RemoveJokeAsync(int id)
        {
            User localUser = DatabaseHandler.GetFromDb(Context.User);
            Joke joke = DatabaseHandler.GetFromDb(new Joke { Id = id }, localUser);

            if (joke is null)
            {
                await ReplyAsync(Message.Error.NotTheOwner);
                return;
            }

            switch (DatabaseHandler.RemoveFromDb(joke))
            {
                case DatabaseHandler.Result.Successful:
                    await ReplyAsync(Message.Info.SuccessfullyRemoved(new Meme()));
                    break;
                default:
                    await ReplyAsync(Message.Error.Generic);
                    break;
            }
        }

        [Command("meme")]
        [Summary("Delete a meme")]
        public async Task RemoveMemeAsync(int id)
        {
            User localUser = DatabaseHandler.GetFromDb(Context.User);
            Meme meme = DatabaseHandler.GetFromDb(new Meme() { Id = id }, localUser);

            if (meme is null)
            {
                await ReplyAsync(Message.Error.NotTheOwner);
                return;
            }

            switch (DatabaseHandler.RemoveFromDb(meme))
            {
                case DatabaseHandler.Result.Successful:
                    await ReplyAsync(Message.Info.SuccessfullyRemoved(new Meme()));
                    break;
                default:
                    await ReplyAsync(Message.Error.Generic);
                    break;
            }
        }

        [Command("reminder", RunMode = RunMode.Async)]
        [Summary("Remove a reminder")]
        public async Task RemoveReminderAsync(int id)
        {
            User localUser = DatabaseHandler.GetFromDb(Context.User);
            Reminder localReminder = DatabaseHandler.GetFromDb(new Reminder() { Id = id }, localUser);

            switch (DatabaseHandler.RemoveFromDb(localReminder))
            {
                case DatabaseHandler.Result.Successful:
                    await ReplyAsync(Message.Info.SuccessfullyRemoved(new Reminder()));
                    break;
                default:
                    await ReplyAsync(Message.Error.Generic);
                    break;
            }
        }

        [Command("alert", RunMode = RunMode.Async)]
        [Summary("Removes an alert, must specify a target user")]
        public async Task RemoveAlertAsync(SocketUser user = null, [Remainder] string etc = "")
        {
            User localUserAuthor = DatabaseHandler.GetFromDb(Context.User);

            if (user is null)
            {
                await ReplyAsync(Message.Error.IncorrectStructure(new Alert()));
                return;
            }

            User localUserTarget = DatabaseHandler.GetFromDb(user);

            Alert n = new Alert
            {
                User = localUserAuthor,
                TargetUser = localUserTarget
            };

            switch (DatabaseHandler.RemoveFromDb(n))
            {
                case DatabaseHandler.Result.Successful:
                    await ReplyAsync(Message.Info.SuccessfullyRemoved(n));
                    break;
                default:
                    await ReplyAsync(Message.Error.Generic);
                    break;
            }
        }
    }

    [Group("set")]
    public class SetGeneralModule : ModuleBase<SocketCommandContext>
    {
        [Command("timezone", RunMode = RunMode.Async)]
        [Summary("Set your timezone")]
        public async Task SetTimezoneAsync(int offset)
        {
            User localUser = DatabaseHandler.GetFromDb(Context.User);

            localUser.TimezoneOffset = offset;
            DatabaseHandler.UpdateInDb(localUser);

            await ReplyAsync(Message.Info.Timezone(localUser));
        }
    }
}
