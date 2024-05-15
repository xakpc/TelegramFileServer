<div align="center">
  <a href="https://github.com/github_username/repo_name">
    <img src="banner.png" alt="Logo">
  </a>

<h3 align="center">Telegram File Server</h3>

  <p align="center">
    REST-based file server API that uses Telegram Bot as storage
    <br />
    <br />
    <a href="https://tg-fs.xakpc.info/index.html">View Demo</a>
    ·
    <a href="https://twitter.com/intent/follow?screen_name=xakpc">Follow @xakpc on X</a>    
  </p>
</div>

Telegram Cloud apparently could provide unlimited storage.

This Proof of Concept is a simple REST-based file server that uses Telegram Cloud as storage.

## How to use

1. Create a new Telegram Bot using [@BotFather](https://t.me/botfather) and get the token.			
1. Initialize the bot by sending `/start` to the bot.			
1. Mute the bot to avoid spamming.							
1. Obtain chat id - one of the ways is to use the following third-party bot [@RawDataBot](https://t.me/RawDataBot)		
1. Run the app and use chat id and token as headers to upload file.

## Deployment

Currently deployed on Azure: [tg-fs.xakpc.info](https://tg-fs.xakpc.info/index.html)