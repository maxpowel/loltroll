What is this?
=============


A tool that helps you to avoid play with trolls in your team. 


How does it work?
=================

When you are in select champ room, the application takes a snap of this window and gets the names of your team summoners. That task is done by an OCR (a tool that read words from images). This is how the summoners names are fetched (not using any api neither patching the game or something weird).

Usage
=====

1. In LolClient go to the queue an wait until you are into the chat room

2. Now push the button "Get summoners". The 5 text edits will be filled with the summoner names. As the OCR is not perfect, it could fail some letter. In this case you can edit it in the text box and put the right one.

3. Click on the button "Search trolls". If a troll is found, the background of the text edit is red (and green if it is not in your troll list)

4. If trolls found, you can dodge (its better lose 5 league points + 5 min than 20 points + 20 min)


Troll database
==============

To make this tool useful you have to create your own troll database. In the table at the right (with headers "Troll name", "Reason", "Fucktard level") you can add new trolls. The troll name is required and if you put an empty name, that row will be removed (this is the way to remove trolls from your list).

The "reason" (optional) is why you added this troll. For example for going directly to turrets for giving free kills.
The fucktard level is how much this troll is idiot. Maybe a summoner is the trollest and his fucktard level is over 9000 (which means you will never want to play with him) or regular troll. Those two columns are only for help you to decide if dodge or not.

Once you have populated your database, click the button "Save". Next time you open the application, load the database by clicking the button "Load". The database is in JSON format so you can open it with a regultar text editor.

Tesseract OCR
=============
Tesseract OCR is used for read the summoners name from the screen capture. I have trained it using the LolClient font so this OCR is optimized only for LolClient. You can train your own and if you get a better one, would be great if you share it.


Notes
=====

Rito can't ban you for using this, its just an application which never access directly to the game client. It just takes a snapshot. They have no way to know if you are using this tool.

Do you think I'm triying to steal your account? No, I don't want your noob account but if you don't trust me, get the code and compile it by yourself.

This application NEVER gets connected to internet. Maybe, in a future, you will be able to save your troll list in the cloud (or someting else) but not now.

If you find some bug or feature request please open an issue.