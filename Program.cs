using System;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Microsoft.VisualBasic.FileIO;

namespace DiscordAddonRimworldTogether
{
    internal class Program
    {
        // shitty code iknow
        public static string? webhookUrl;
        public static string? serverLaunchedMessage;
        public static string? userJoinMessage;
        public static string? userLeftMessage;
        public static string? broadcastMessage;
        public static bool chat;
        public static string? newSettlementMessage;
        public static string? newFaction;
        public static string? deletedFaction;
        public static string configPath = "Core/RTDiscordConfig.json";

        static void Main()
        {
            var defaultConfig = new
            {
                webhook_url = "",
                message = new
                {
                    server_launched = "Server started",
                    user_join = "%user% connected to the server",
                    user_disconnect = "%username% disconnected from the server",
                    broadcast = "Broadcast: %broadcast%",
                    chat_messages = true,
                    new_settlement_message = "New settlement: %settlement%",
                    created_faction = "New faction created: %faction_name%",
                    deleted_faction = "Faction %faction_name% was deleted from the world"
                }
            };

            // Loading configuration
            if (!File.Exists(configPath))
            {
                string jsonConfig = JsonConvert.SerializeObject(defaultConfig, Newtonsoft.Json.Formatting.Indented);
                Directory.CreateDirectory(Path.GetDirectoryName(configPath));
                File.WriteAllText(configPath, jsonConfig);
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss]") + " Creating config file " + configPath);
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss]") + " Loading configuration " + configPath);
                loadConfigVars();
                CreateDefaultConfig(configPath);
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss]") + " Loading configuration " + configPath);
                loadConfigVars();
            }

            // Starting server 
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = @"GameServer.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + " Loaded discord server addon");
                StreamReader reader = process.StandardOutput;

                while (!process.HasExited)
                {
                    string output = reader.ReadLine();

                    if (output != null)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(output);

                        if (output.Contains("Loaded optional mods"))
                        {
                            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "| Reloaded discord server addon", Console.ForegroundColor = ConsoleColor.Blue);
                            Console.ForegroundColor = ConsoleColor.White;
                            loadConfigVars();
                        }

                        // sorry for spaghetti code 
                        if (!webhookUrl.Contains("discord"))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + " Webhook not configurated");
                            return;
                        }

                        if (output.Contains("Server launched"))
                        {
                            if (serverLaunchedMessage != null)
                            {
                                SendMs(serverLaunchedMessage);
                            }
                            else
                            {
                                SendMs("Server launched");
                            }
                        }
                        else if (output.Contains("Handshake"))
                        {
                            string pattern = @">\s*(.*?)\s*\|";
                            Match match = Regex.Match(output, pattern);
                            if (match.Success)
                            {
                                string username = match.Groups[1].Value.Trim();
                                if (userJoinMessage != null)
                                {
                                    SendMs(userJoinMessage.Replace("%user%", username));
                                }
                                else
                                {
                                    SendMs(username + " connected to the server");
                                }
                            }
                        }
                        else if (output.Contains("Sent broadcast"))
                        {
                            string pattern = @"Sent broadcast '([^']+)'";
                            Match match = Regex.Match(output, pattern);
                            if (match.Success)
                            {
                                string broadcast = match.Groups[1].Value.Trim();
                                if (broadcastMessage != null)
                                {
                                    SendMs(broadcastMessage.Replace("broadcast", broadcast));
                                }
                                else
                                {
                                    SendMs("Broadcast message: " + broadcast);
                                }

                            }
                        }
                        else if (output.Contains("Disconnect"))
                        {
                            string pattern = @">\s*(.*?)\s*\|";
                            Match match = Regex.Match(output, pattern);
                            if (match.Success)
                            {
                                string username = match.Groups[1].Value.Trim();
                                if (userLeftMessage != null)
                                {
                                    SendMs(userLeftMessage.Replace("%user%", username));
                                }
                                else
                                {
                                    SendMs(username + "left the server");
                                }
                            }
                        }
                        else if (output.Contains("Chat"))
                        {
                            if (chat)
                            {
                                string regex = @"\[Chat\] > (\S+) > (.+)";
                                Match match = Regex.Match(output, regex);
                                if (match.Success)
                                {
                                    string username = match.Groups[1].Value;
                                    string message = match.Groups[2].Value;
                                    SendMs($"[Chat] {username}: {message}");
                                } else
                                {
                                    SendMs(output);
                                }
                            }
                        }
                        else if (output.Contains("Added settlement"))
                        {
                            string pattern = @">\s*(.*?)\s*\|";
                            Match match = Regex.Match(output, pattern);
                            if (match.Success)
                            {
                                string settlement = match.Groups[1].Value.Trim();
                                SendMs(newSettlementMessage.Replace("%settlement%", settlement));
                            } else
                            {
                                SendMs(output);
                            }
                        }
                        else if (output.Contains("Created faction"))
                        {
                            string pattern = @">\s*(.*?)\s*\|";
                            Match match = Regex.Match(output, pattern);
                            if (match.Success)
                            {
                                string factionName = match.Groups[1].Value.Trim();
                                SendMs(newFaction.Replace("%faction_name%", factionName));
                            } else
                            {
                                SendMs(output);
                            }
                        }
                        else if (output.Contains("Deleted faction"))
                        {
                            string pattern = @">\s*(.*?)\s*\|";
                            Match match = Regex.Match(output, pattern);
                            if (match.Success)
                            {
                                string factionName = match.Groups[1].Value.Trim();
                                SendMs(deletedFaction.Replace("%faction_name%", factionName));
                            } else
                            {
                                SendMs(output);
                            }
                        }
                    }
                }

                process.WaitForExit();
            }
            catch (Exception error)
            {
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Error: " + error);
                Console.ReadLine();
            }
        }

        // Webhook ms system (thanks stackoverflow)
        static void SendMs(string message)
        {
            try
            {
                string webhook = webhookUrl;
                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                string payload = "{\"content\": \"" + DateTime.Now.ToString("[HH:mm:ss] ") + message + "\"}";
                client.UploadData(webhook, Encoding.UTF8.GetBytes(payload));
            }
            catch (Exception error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Error: " + error);
                Console.WriteLine("Maybe fix: You must put this file in the same file that GameServer.exe");
            }
        }

        static void loadConfigVars()
        {
            var config = LoadConfig(configPath);
            webhookUrl = config.webhook_url;
            serverLaunchedMessage = config.message.server_launched;
            userJoinMessage = config.message.user_join;
            userLeftMessage = config.message.user_disconnect;
            broadcastMessage = config.message.broadcast_message;
            newSettlementMessage = config.message.new_settlement_message;
            chat = config.message.chat_messages;
            newFaction = config.message.created_faction;
            deletedFaction = config.message.deleted_faction;
        }

        static void CreateDefaultConfig(string filePath)
        {
            var defaultConfig = new
            {
                webhook_url = "",
                message = new
                {
                    server_launched = "Server started",
                    user_join = "%user% connected to the server",
                    user_disconnect = "%user% disconnected from the server",
                    broadcast = "Broadcast: %broadcast%",
                    chat_messages = true,
                    new_settlement_message = "New settlement: %settlement%",
                    created_faction = "New faction created: %faction_name%",
                    deleted_faction = "Faction %faction_name% was deleted from the world"
                }
            };
            string jsonConfig = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
            File.WriteAllText(filePath, jsonConfig);
        }

        static Config LoadConfig(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Config config = JsonConvert.DeserializeObject<Config>(json);
            return config;
        }

    }

    class Config
    {
        public string webhook_url { get; set; }
        public MessageConfig message { get; set; }
    }

    class MessageConfig
    {
        public string server_launched { get; set; }
        public string user_join { get; set; }
        public string user_disconnect { get; set; }
        public string broadcast_message { get; set; }
        public bool chat_messages { get; set; }
        public string new_settlement_message { get; set; }
        public string created_faction { get; set; }
        public string deleted_faction { get; set; }

    }
}
