﻿using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Auditor
{
    public class AuditorModule : ModuleBase<ShardedCommandContext>
    {
        protected async Task<IUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false,
            Embed embed = null, RequestOptions options = null, bool isSpoiler = false)
        {
            return await Context.Channel.SendFileAsync(filePath, text, isTTS, embed, options, isSpoiler);
        }

        protected async Task<IUserMessage> SendFileAsync(Stream stream, string filename, string text = null,
            bool isTts = false, Embed embed = null, RequestOptions options = null, bool isSpoiler = false)
        {
            return await Context.Channel.SendFileAsync(stream, filename, text, isTts, embed, options, isSpoiler);
        }

        protected async Task<IUserMessage> SendEmbedAsync(Embed embed)
        {
            return await Context.Channel.SendMessageAsync("", false, embed);
        }

        protected async Task<IUserMessage> SendErrorAsync(string error)
        {
            return await SendErrorAsync("Error", error);
        }

        protected async Task<IUserMessage> SendErrorAsync(string title, string description)
        {
            Embed embed = new EmbedBuilder()
                .WithTitle($"❌ {title} ❌")
                .WithDescription(description)
                .WithColor(Color.Teal)
                .Build();

            return await Context.Channel.SendMessageAsync("", false, embed);
        }

        protected async Task<IUserMessage> SendSuccessAsync(string success)
        {
            Embed embed = new EmbedBuilder()
                .WithTitle("☑ Success ☑")
                .WithDescription(success)
                .WithColor(Color.Teal)
                .Build();

            return await Context.Channel.SendMessageAsync("", false, embed);
        }

        protected async Task<IUserMessage> SendWarningAsync(string warning)
        {
            Embed embed = new EmbedBuilder()
                .WithTitle("❗ Warning ❗")
                .WithDescription(warning)
                .WithColor(Color.Teal)
                .Build();

            return await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}