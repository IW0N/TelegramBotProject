# TelegramBotProject
<p>
This is project of telegram bot "Роспросветнадзор"</br>
This bot is needed to provide access to large articles in telegram.</br>
It divides users into several types:
<ul>
<li><b>Regular user (reader)<b></li>
<li><b>Article redactor<b></li>
<li>Administrator of redactors</li>
<li>Bot owner</li>
</ul>
</p>
<h2>About roles</h2>
<h3>Regular user</h3>
<p>He can read articles only and post request to be redactor or adminstrator if sending of post mode is activity.</br>
Commands:
<ul>
<li>/start - atricle list</li>
<li>/help - all commands to you</li>
<li>/getviews - see views of atricles</li>
<li>/cancel - cancel command execution</li>
<li>/besuperuser - Send request to super user role</li>
<li>/state_of_besuperuser - Find out if it is possible to submit an application for the role of superuser</li>
</ul>
</p>
<h3>Article redactor</h3>
<p>This is the most important type. He can redact articles, rename it and remove if other redactor or person with rights up then himself will not recover post</br>
Acess commands:
<ul>
<li>Commands of ordinay user</li>
<li>/createPost - create article with name, url and description</li>
<li>/rename - rename article</li>
<li>/editDescription - edit article description</li>
<li>/editUrl - edit article url</li>
<li>/removePost - remove article</li>
<li>/exit - stop be super user</li>
<li>/getSuperList - view super user list</li>
<li>/aboutMe - view information about self and your role</li>
<li>/sendMessage - send message to other super user.</br>PS You can attach no more, than one photo or document to message</li>
</ul>
</p>
<h3>Administrator</h3>
<p>This is 'police' of bot. They watch to no one redactor don't publish delusion. In other case this redactor will be ban</p>
<p>
Commands:
<ul>
<li>Commands of ordinary user and redctor</li>
<li>/addNewSuperUser - accept or deny user be super user request</li>
<li>/ban - ban redactor</li>
<li>/recoverSuperUser - recover redactor, explaining situation</li>
</ul>
</p>
<h3>Owner</h3>
<p>Sometime administrators execute self work badly. In this case you can ban administrator. Also you can on/off recruitment of new super users</br>
Commands:
<ul>
<li>Commands of regular user, redactor and administrator</li>
<li>/editRightLevel - allow to increase or decrease in rights</li>
<li>/switchNSUA - switch new super user acceptance</li>
</ul>
</p>
