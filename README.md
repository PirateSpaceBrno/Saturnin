# Saturnin
Saturnin is bot for Signal messaging service written in C# for Mono framework (running on Linux) using https://github.com/AsamK/signal-cli (DBus interface) as Signal client.

In the first place, it's mentioned to be used by members of Czech Pirate party which are using Signal for daily internal communication.

Actually it offers you some commands recognized by sent phrase (in Czech language) on specified number, which is connected via Signal-cli to Signal messaging service.

The list of used public APIs:
- GrapAPI (https://graph.pirati.cz/) - used to return groups and count of its members from the forum of Czech Pirate party
- Šotoris API (http://sotoris.cz/DataSource/CityHack2015/) - used to follow vehicles of public transport in Brno (Czech Republic) published
- Lamer.cz (http://lamer.cz/) - parsing random jokes from this webpage

Saturnin also offers:
- scheduled Signal messages - You can schedule message with your text to be send to another user of Signal service. Also, you can schedule message for yourself, which makes Saturnin your small task manager.
