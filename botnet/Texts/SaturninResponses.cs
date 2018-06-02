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
            $"{Configuration.SALUTATION}, pozdrav! | vypíše informace o botovi\n" +
            $"{Configuration.SALUTATION}, kolik členů má SKUPINA? | vrátí počet členů definované skupiny z Pirátského fóra\n" +
            $"{Configuration.SALUTATION}, řekni vtip. | vrátí náhodný vtip ze stránky Lamer.cz\n" +
            ""
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
    }
}
