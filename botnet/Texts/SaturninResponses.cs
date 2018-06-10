using System.IO;
using System.Reflection;

namespace Saturnin.Texts
{
    public static class SaturninResponses
    {
        public static string Help => 
            "Saturnin - Pirátský bot pro síť Signal\n" +
            "--------------------------------------\n" +
            $"{Configuration.SALUTATION}? | zobrazí tuto nápovědu\n" +
            "\n" +
            $"{Configuration.SALUTATION}, pozdrav! | vypíše informace o botovi\n" +
            "\n" +
            $"{Configuration.SALUTATION}, pozdrav uživatele +420123456789 | zašle uvítací zprávu uživateli Signalu na čísle +420123456789\n" +
            "\n" +
            $"{Configuration.SALUTATION}, kolik členů má SKUPINA? | vrátí počet členů definované skupiny z Pirátského fóra\n" +
            "\n" +
            $"{Configuration.SALUTATION}, vypiš všechny skupiny. | vrátí seznam všech skupin z Pirátského fóra\n" +
            "\n" +
            $"{Configuration.SALUTATION}, řekni vtip | vrátí náhodný vtip ze stránky Lamer.cz\n" +
            "\n" +
            $"{Configuration.SALUTATION}, ve ČAS pošli zprávu s textem 'TEXT' na číslo +420123456789 | vytvoří plánovanou zprávu\n" +
            "\n" +
            $"{Configuration.SALUTATION}, ve ČAS mi pošli zprávu s textem 'TEXT' | vytvoří plánovanou zprávu sama sobě\n" +
            "\n" +
            $"{Configuration.SALUTATION}, zruš mé naplánované zprávy | odstraní všechny naplánované zprávy\n" +
            "\n" +
            $"{Configuration.SALUTATION}, kolik mám naplánovaných zpráv? | vypíše počet aktuálně naplánovaných zpráv\n" +
            "\n" +
            $"{Configuration.SALUTATION}, sleduj DPMB linku 123 v okruhu 200 metrů od bodu 49.19083,16.60498 | pošle zprávu, když se nějaký vůz DPMB linky 123 bude nacházet 200 metrů od bodu zadaného souřadnicemi\n" +
            "\n" +
            $"{Configuration.SALUTATION}, přestaň sledovat DPMB linku 123 | přestane zasílat informace o poloze DPMB linky 123\n" +
            "\n" +
            $"{Configuration.SALUTATION}, přestaň sledovat všechny DPMB linky | přestane zasílat informace o poloze všech odebíraných linek DPMB\n" +
            "\n" +
            $"{Configuration.SALUTATION}, jaké sleduješ DPMB linky? | pošle seznam sledovaných linek DPMB\n" +
            "\n"
        ;

        public static string Hello =>
            "Ahoj, já jsem Saturnin.\n" +
            "Prozatím jsem ve vývoji, ale postupně se učím nové dovednosti.\n" +
            "Pokud se o mě chceš dozvědět více nebo se připojit k mému vývoji, navštiv stránku http://saturnin.piratsky.space/. Jsem svobodný, zveřejněný pod licencí GNU GPLv3 a mé zdrojové kódy najdeš na https://github.com/PirateSpaceBrno/Saturnin. \n" +
            "\n" +
            "Zašli odpověď \"Saturnine?\" (bez uvozovek) pro získání seznamu dostupných příkazů.\n" +
            "\n" +
            $"Datum mého posledního vylepšení: {new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime.ToString("dd.MM.yyyy")}"
        ;

        public static string Wait =>
            "Moment, přemýšlím..."
        ;

        public static string ScheduleMessageForMe =>
            $@"(({Configuration.SALUTATION}, (v|ve)) (.*) mi po(s|š)li zpr(a|á)vu s textem '(.*)')"
        ;

        public static string ScheduleMessage =>
            $@"(({Configuration.SALUTATION}, (v|ve)) (.*) po(s|š)li zpr(a|á)vu s textem '(.*)' na (ci|čí)slo (.*))"
        ;
    }
}
