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
            "\n"
        ;

        public static string Hello =>
            "Ahoj, já jsem Saturnin.\n" +
            "Prozatím jsem ve vývoji, ale postupně se učím nové dovednosti.\n" +
            "Pokud se o mě chceš dozvědět více nebo se připojit k mému vývoji, navštiv stránku http://saturnin.piratsky.space/. Jsem svobodný, zveřejněný pod licencí GNU GPLv3 a mé zdrojové kódy najdeš na https://github.com/PirateSpaceBrno/Saturnin. \n" +
            "\n" +
            $"Naposledy mě vylepšili {new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime}"
        ;

        public static string Wait =>
            "Moment, přemýšlím..."
        ;

        public static string ScheduleMessageForMe =>
            $@"(({Configuration.SALUTATION}|{Configuration.SALUTATION.ToLowerInvariant()}), (v|ve) (.*) mi po(s|š)li zpr(a|á)vu s textem '(.*)')"
        ;

        public static string ScheduleMessage =>
            $@"(({Configuration.SALUTATION}|{Configuration.SALUTATION.ToLowerInvariant()}), (v|ve) (.*) po(s|š)li zpr(a|á)vu s textem '(.*)' na (ci|čí)slo (.*))"
        ;
    }
}
