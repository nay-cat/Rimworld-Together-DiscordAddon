# Rimworld Together Discord Integration
> Discord webhook extension for Rimworld Together

![image](https://github.com/nay-cat/Rimworld-Together-DiscordAddon/assets/63517637/834b6611-25fb-4247-836b-42a1857a5230)

> [Rimworld Together Server Download](https://github.com/RimworldTogether/Rimworld-Together)
> [Rimworld Together Mod Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3005289691)

## How to use

> Drag the downloaded files into the server folder
![image](https://github.com/nay-cat/Rimworld-Together-DiscordAddon/assets/63517637/7d1a1b20-b9cc-4689-8799-ae66568e7277)

> Open `DiscordAddonRimworldTogether.exe`
![image](https://github.com/nay-cat/Rimworld-Together-DiscordAddon/assets/63517637/22323783-0eac-4f28-b60d-46246eb3500c)

> You will see this
![image](https://github.com/nay-cat/Rimworld-Together-DiscordAddon/assets/63517637/11af317e-15cb-4931-8962-c43730cffc80)

> Go to your desired channel and create a webhook, then copy the url

[Creating webhook tutorial](https://images.ctfassets.net/qqlj6g4ee76j/asset-c4b59737607fde207c9dc742adc2ade7/8a78e081ed1eb2549e2c1c147ec0248c/creating-a-discord-webhook-3.png)

![image](https://github.com/nay-cat/Rimworld-Together-DiscordAddon/assets/63517637/9559a0a8-0745-4eab-9a68-62556745eaea)

> Edit "webhook_url" at Core/RTDiscordConfig.json and save it

![image](https://github.com/nay-cat/Rimworld-Together-DiscordAddon/assets/63517637/a67d11d8-5d55-4e8b-a8c3-4d5e21ed0519)

![image](https://github.com/nay-cat/Rimworld-Together-DiscordAddon/assets/63517637/9bc29bf0-9428-4e74-8f6c-7180c28a2ec7)

> Now open `DiscordAddonRimworldTogether.exe` every time that you want to start the server with discord integration

![image](https://github.com/nay-cat/Rimworld-Together-DiscordAddon/assets/63517637/b27125d7-2537-4f80-8891-4617a8a4b553)

## Config

> Reload command also reload the configuration

![image](https://github.com/nay-cat/Rimworld-Together-DiscordAddon/assets/63517637/64d8889f-adeb-494f-94d9-d9000de01db9)


```json
{
  "webhook_url": "",
  "message": {
    "server_launched": "Server started",
    "user_join": "%user% connected to the server",
    "user_disconnect": "%user% disconnected from the server",
    "broadcast": "Broadcast: %broadcast%",
    "chat_messages": true,
    "new_settlement_message": "New settlement: %settlement%",
    "created_faction": "New faction created: %faction_name%",
    "deleted_faction": "Faction %faction_name% was deleted from the world"
  }
}
```


