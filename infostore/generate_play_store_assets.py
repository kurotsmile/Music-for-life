from __future__ import annotations

from pathlib import Path
from textwrap import dedent

from PIL import Image, ImageDraw, ImageFilter, ImageFont


SCRIPT_DIR = Path(__file__).resolve().parent
WORKSPACE_ROOT = SCRIPT_DIR.parent if SCRIPT_DIR.name == "infostore" else SCRIPT_DIR
INFOSTORE_DIR = WORKSPACE_ROOT / "infostore"
LANG_DIR = WORKSPACE_ROOT / "Assets" / "Resources" / "LangCountry"
APP_NAME = "Heartbeat Music"


LISTING_DATA = {
    "en": {
        "short": "Stream music, radio, ambient sounds, and offline playlists in one app.",
        "full": dedent(
            """
            Heartbeat Music brings together online songs, internet radio, relaxing sound collections, and your own offline library in one easy player.

            Discover music by language, artist, album, genre, or year. Search tracks quickly, open song links, share favorites, and view lyrics when available. Save songs, radio stations, and sound playlists to your personal offline collection so you can listen again without starting from zero.

            Import songs from your device or an entire folder, organize content into playlists, and back up your library with JSON export and import tools. Heartbeat Music also includes a full player with playlist queue support, loop controls, audio mixer options, and a visualizer mode for a more immersive listening experience.

            Whether you want online discovery, radio streaming, relaxing background audio, or your own saved playlists, Heartbeat Music keeps everything in one place.
            """
        ).strip(),
    },
    "vi": {
        "short": "Nghe nhạc, radio, âm thanh thư giãn và playlist offline trong một app.",
        "full": dedent(
            """
            Heartbeat Music kết hợp nhạc online, radio internet, âm thanh thư giãn và thư viện offline cá nhân trong một trình phát dễ dùng.

            Khám phá bài hát theo ngôn ngữ, nghệ sĩ, album, thể loại hoặc năm phát hành. Tìm kiếm nhanh, mở liên kết bài hát, chia sẻ bản nhạc yêu thích và xem lời bài hát khi có sẵn. Bạn cũng có thể lưu bài hát, kênh radio và danh sách âm thanh vào bộ sưu tập offline để nghe lại bất cứ lúc nào.

            Ứng dụng cho phép nhập nhạc từ thiết bị hoặc cả thư mục, sắp xếp nội dung thành playlist và sao lưu kho nhạc bằng công cụ xuất nhập JSON. Heartbeat Music còn có hàng đợi phát, chế độ lặp, bộ trộn âm thanh và visualizer giúp trải nghiệm nghe nhạc sống động hơn.

            Nếu bạn cần một ứng dụng để nghe nhạc online, nghe radio, mở âm thanh nền thư giãn hoặc quản lý danh sách phát đã lưu, Heartbeat Music giữ mọi thứ gọn trong cùng một nơi.
            """
        ).strip(),
    },
    "ar": {
        "short": "استمع إلى الموسيقى والراديو والأصوات الهادئة والقوائم دون اتصال في تطبيق واحد.",
        "full": dedent(
            """
            يجمع Music For Life بين الأغاني عبر الإنترنت، ومحطات الراديو، ومجموعات الأصوات الهادئة، ومكتبتك الخاصة دون اتصال داخل مشغل واحد سهل الاستخدام.

            اكتشف الموسيقى حسب اللغة أو الفنان أو الألبوم أو النوع أو السنة. ابحث عن المقاطع بسرعة، وافتح روابط الأغاني، وشارك المفضلة لديك، واعرض كلمات الأغاني عند توفرها. يمكنك أيضا حفظ الأغاني ومحطات الراديو وقوائم الأصوات في مجموعتك غير المتصلة للاستماع إليها لاحقا بسهولة.

            استورد الأغاني من جهازك أو من مجلد كامل، ونظم المحتوى داخل قوائم تشغيل، واحفظ مكتبتك باستخدام أدوات التصدير والاستيراد بصيغة JSON. كما يوفر Music For Life قائمة تشغيل حالية، وأوضاع تكرار، وخيارات لمزج الصوت، ووضعا بصريا يمنحك تجربة استماع أكثر حيوية.

            سواء كنت تريد اكتشاف موسيقى جديدة، أو بث الراديو، أو تشغيل أصوات هادئة في الخلفية، أو إدارة قوائمك المحفوظة، فإن Music For Life يجمع كل ذلك في مكان واحد.
            """
        ).strip(),
    },
    "cs": {
        "short": "Hudba, radio, relaxacni zvuky a offline playlisty v jedne aplikaci.",
        "full": dedent(
            """
            Music For Life spojuje online skladby, internetove radio, relaxacni zvukove kolekce a vasi vlastni offline knihovnu do jednoho jednoducheho prehravace.

            Objevujte hudbu podle jazyka, interpreta, alba, zanru nebo roku. Rychle vyhledavejte skladby, otevirajte odkazy na pisne, sdilejte oblibene a zobrazujte texty, kdyz jsou k dispozici. Skladby, radia i zvukove playlisty si muzete ulozit do osobni offline sbirky a poslouchat je kdykoli znovu.

            Importujte hudbu ze sveho zarizeni nebo cele slozky, organizujte obsah do playlistu a zalohujte knihovnu pomoci exportu a importu JSON. Music For Life nabizi take frontu prehravani, rezimy opakovani, moznosti audio mixu a vizualizer pro pusobivejsi poslech.

            At uz chcete objevovat hudbu online, poslouchat radio, pustit si zvuky na pozadi nebo spravovat ulozene seznamy, Music For Life drzi vse pohromade na jednom miste.
            """
        ).strip(),
    },
    "da": {
        "short": "Musik, radio, rolige lyde og offline-playlister samlet i en app.",
        "full": dedent(
            """
            Music For Life samler online musik, internetradio, afslappende lydsamlinger og dit eget offlinebibliotek i en enkel afspiller.

            Udforsk musik efter sprog, kunstner, album, genre eller ar. Sog hurtigt efter numre, abn sanglinks, del favoritter og se sangtekster, nar de er tilgaengelige. Du kan ogsa gemme sange, radiostationer og lydplaylister i din personlige offline samling, sa de er klar senere.

            Importer musik fra din enhed eller en hel mappe, organiser indhold i playlister, og sikkerhedskopier biblioteket med JSON-eksport og import. Music For Life giver dig ogsa afspilningsko, loop-funktioner, lydmixer-indstillinger og en visualizer-tilstand for en mere levende lytteoplevelse.

            Uanset om du vil opdage ny musik online, streame radio, afspille rolige baggrundslyde eller styre dine gemte lister, holder Music For Life det hele samlet et sted.
            """
        ).strip(),
    },
    "de": {
        "short": "Musik, Radio, Ambient-Sounds und Offline-Playlists in einer App.",
        "full": dedent(
            """
            Music For Life vereint Online-Musik, Internetradio, entspannende Klangsammlungen und Ihre eigene Offline-Bibliothek in einem leicht bedienbaren Player.

            Entdecken Sie Musik nach Sprache, Interpret, Album, Genre oder Jahr. Suchen Sie Titel schnell, offnen Sie Song-Links, teilen Sie Favoriten und lesen Sie Liedtexte, wenn sie verfugbar sind. Sie konnen Songs, Radiosender und Sound-Playlists auch in Ihrer personlichen Offline-Sammlung speichern und spater bequem wiedergeben.

            Importieren Sie Musik von Ihrem Gerat oder aus ganzen Ordnern, organisieren Sie Inhalte in Playlists und sichern Sie Ihre Bibliothek per JSON-Export und -Import. Music For Life bietet zudem eine Warteschlange, Wiederholungsmodi, Audio-Mixer-Optionen und einen Visualizer fur ein intensiveres Horerlebnis.

            Ob Sie online neue Musik entdecken, Radio streamen, ruhige Hintergrundklange abspielen oder gespeicherte Listen verwalten mochten: Music For Life bringt alles an einem Ort zusammen.
            """
        ).strip(),
    },
    "es": {
        "short": "Musica, radio, sonidos relajantes y listas offline en una sola app.",
        "full": dedent(
            """
            Music For Life une canciones online, radio por internet, colecciones de sonidos relajantes y tu propia biblioteca sin conexion en un reproductor facil de usar.

            Descubre musica por idioma, artista, album, genero o ano. Busca canciones rapidamente, abre enlaces, comparte tus favoritas y consulta letras cuando esten disponibles. Tambien puedes guardar canciones, emisoras de radio y listas de sonidos en tu coleccion offline para volver a escucharlas cuando quieras.

            Importa musica desde tu dispositivo o desde una carpeta completa, organiza el contenido en playlists y respalda tu biblioteca con herramientas de exportacion e importacion JSON. Music For Life tambien incluye cola de reproduccion, modos de repeticion, opciones de mezclador de audio y un visualizador para una experiencia mas inmersiva.

            Ya sea para descubrir musica online, escuchar radio, reproducir sonidos de fondo relajantes o gestionar tus listas guardadas, Music For Life mantiene todo en un solo lugar.
            """
        ).strip(),
    },
    "fi": {
        "short": "Musiikki, radio, rauhoittavat aanet ja offline-listat yhdessa sovelluksessa.",
        "full": dedent(
            """
            Music For Life yhdistaa verkkomusiikin, internetradion, rentouttavat aanikokoelmat ja oman offline-kirjastosi yhteen helppokayttoiseen soittimeen.

            Loyda musiikkia kielen, artistin, albumin, genren tai vuoden mukaan. Hae kappaleita nopeasti, avaa linkkeja, jaa suosikkeja ja nayta sanoitukset, kun ne ovat saatavilla. Voit myos tallentaa kappaleita, radioasemia ja aanilistoja omaan offline-kokoelmaasi, jotta ne ovat helposti saatavilla myohemmin.

            Tuo musiikkia laitteeltasi tai kokonaisesta kansiosta, jarjesta sisalto soittolistoihin ja varmuuskopioi kirjastosi JSON-viennilla ja -tuonnilla. Music For Life tarjoaa lisaksi toistojonon, toistotilat, aanimikserin asetukset ja visualisointitilan elavaisempaa kuuntelua varten.

            Halusitpa loytaa uutta musiikkia verkosta, kuunnella radiota, kayttaa rauhoittavia taustaaania tai hallita tallennettuja listoja, Music For Life kokoaa kaiken yhteen paikkaan.
            """
        ).strip(),
    },
    "fr": {
        "short": "Musique, radio, sons relaxants et playlists hors ligne dans une seule app.",
        "full": dedent(
            """
            Music For Life rassemble les chansons en ligne, la radio internet, des collections de sons relaxants et votre bibliotheque hors ligne dans un lecteur simple a utiliser.

            Decouvrez la musique par langue, artiste, album, genre ou annee. Recherchez rapidement des titres, ouvrez des liens, partagez vos favoris et affichez les paroles lorsqu'elles sont disponibles. Vous pouvez aussi enregistrer chansons, radios et listes de sons dans votre collection hors ligne personnelle pour les retrouver facilement plus tard.

            Importez de la musique depuis votre appareil ou un dossier complet, organisez le contenu en playlists et sauvegardez votre bibliotheque grace aux outils d'export et d'import JSON. Music For Life propose egalement une file de lecture, des modes de repetition, des options de mixage audio et un visualiseur pour une ecoute plus immersive.

            Que vous vouliez decouvrir de la musique en ligne, ecouter la radio, lancer des sons d'ambiance ou gerer vos listes sauvegardees, Music For Life garde tout au meme endroit.
            """
        ).strip(),
    },
    "hi": {
        "short": "एक ही ऐप में संगीत, रेडियो, शांत ध्वनियां और ऑफलाइन प्लेलिस्ट.",
        "full": dedent(
            """
            Music For Life ऑनलाइन गाने, इंटरनेट रेडियो, सुकून देने वाली ध्वनियों के संग्रह और आपकी अपनी ऑफलाइन लाइब्रेरी को एक आसान प्लेयर में साथ लाता है.

            भाषा, कलाकार, एल्बम, शैली या वर्ष के आधार पर संगीत खोजें. गानों को जल्दी खोजें, लिंक खोलें, पसंदीदा साझा करें और उपलब्ध होने पर लिरिक्स देखें. आप गाने, रेडियो स्टेशन और साउंड प्लेलिस्ट को अपनी ऑफलाइन कलेक्शन में सेव भी कर सकते हैं ताकि बाद में आसानी से सुन सकें.

            अपने डिवाइस या पूरे फोल्डर से गाने इम्पोर्ट करें, कंटेंट को प्लेलिस्ट में व्यवस्थित करें और JSON एक्सपोर्ट तथा इम्पोर्ट टूल से लाइब्रेरी का बैकअप लें. Music For Life में प्लेबैक क्यू, लूप मोड, ऑडियो मिक्सर विकल्प और विजुअलाइज़र मोड भी है, जो सुनने का अनुभव और बेहतर बनाता है.

            चाहे आप नया ऑनलाइन संगीत खोजना चाहते हों, रेडियो स्ट्रीम करना चाहते हों, बैकग्राउंड में शांत ध्वनियां चलाना चाहते हों या अपनी सेव की हुई सूचियां संभालना चाहते हों, Music For Life सब कुछ एक ही जगह रखता है.
            """
        ).strip(),
    },
    "id": {
        "short": "Musik, radio, suara relaksasi, dan playlist offline dalam satu aplikasi.",
        "full": dedent(
            """
            Music For Life menggabungkan lagu online, radio internet, koleksi suara relaksasi, dan pustaka offline pribadi Anda dalam satu pemutar yang mudah digunakan.

            Temukan musik berdasarkan bahasa, artis, album, genre, atau tahun. Cari lagu dengan cepat, buka tautan lagu, bagikan favorit, dan lihat lirik saat tersedia. Anda juga bisa menyimpan lagu, stasiun radio, dan playlist suara ke koleksi offline pribadi agar mudah diputar kembali kapan saja.

            Impor musik dari perangkat Anda atau dari satu folder penuh, atur konten ke dalam playlist, dan cadangkan pustaka dengan alat ekspor dan impor JSON. Music For Life juga menyediakan antrean putar, mode loop, opsi audio mixer, dan mode visualizer untuk pengalaman mendengarkan yang lebih hidup.

            Baik Anda ingin menemukan musik online, mendengarkan radio, memutar audio latar yang menenangkan, atau mengelola daftar simpanan, Music For Life menyatukan semuanya dalam satu tempat.
            """
        ).strip(),
    },
    "it": {
        "short": "Musica, radio, suoni rilassanti e playlist offline in un'unica app.",
        "full": dedent(
            """
            Music For Life riunisce canzoni online, radio via internet, raccolte di suoni rilassanti e la tua libreria offline personale in un lettore semplice da usare.

            Scopri la musica per lingua, artista, album, genere o anno. Cerca rapidamente i brani, apri i link delle canzoni, condividi i preferiti e visualizza i testi quando disponibili. Puoi anche salvare canzoni, stazioni radio e playlist di suoni nella tua raccolta offline personale per ascoltarli di nuovo in qualsiasi momento.

            Importa musica dal tuo dispositivo o da un'intera cartella, organizza i contenuti in playlist ed esegui il backup della libreria con gli strumenti di esportazione e importazione JSON. Music For Life include anche coda di riproduzione, modalita loop, opzioni di audio mixer e una modalita visualizer per un ascolto piu coinvolgente.

            Che tu voglia scoprire musica online, ascoltare la radio, riprodurre suoni di sottofondo rilassanti o gestire le tue liste salvate, Music For Life mantiene tutto in un unico posto.
            """
        ).strip(),
    },
    "ja": {
        "short": "音楽、ラジオ、環境音、オフライン再生を1つのアプリで楽しめます。",
        "full": dedent(
            """
            Music For Life は、オンライン楽曲、インターネットラジオ、リラックスできるサウンド、そして自分のオフラインライブラリを、使いやすい1つのプレーヤーにまとめます。

            言語、アーティスト、アルバム、ジャンル、年代から音楽を探せます。曲をすばやく検索し、リンクを開き、お気に入りを共有し、利用できる場合は歌詞も表示できます。曲、ラジオ局、サウンドプレイリストを自分用のオフラインコレクションに保存して、後から簡単に再生できます。

            端末内の曲やフォルダ単位の音楽を取り込み、プレイリストで整理し、JSON のエクスポートとインポートでライブラリをバックアップできます。Music For Life には、再生キュー、ループ設定、オーディオミキサー、より没入感のある視聴を楽しめるビジュアライザーモードも用意されています。

            オンラインで新しい音楽を見つけたいときも、ラジオを聴きたいときも、落ち着いた背景音を流したいときも、保存済みリストを管理したいときも、Music For Life ならすべてを1か所でまとめて使えます。
            """
        ).strip(),
    },
    "ko": {
        "short": "음악, 라디오, 배경 사운드, 오프라인 재생목록을 한 앱에서.",
        "full": dedent(
            """
            Music For Life는 온라인 음악, 인터넷 라디오, 편안한 사운드 컬렉션, 그리고 나만의 오프라인 라이브러리를 하나의 사용하기 쉬운 플레이어로 모아 줍니다.

            언어, 아티스트, 앨범, 장르, 연도별로 음악을 찾아보세요. 곡을 빠르게 검색하고, 링크를 열고, 즐겨찾기를 공유하고, 제공되는 경우 가사도 볼 수 있습니다. 노래, 라디오 방송국, 사운드 재생목록을 개인 오프라인 컬렉션에 저장해 두고 나중에 다시 쉽게 들을 수 있습니다.

            기기나 전체 폴더에서 음악을 가져오고, 재생목록으로 정리하고, JSON 내보내기와 가져오기 도구로 라이브러리를 백업할 수 있습니다. Music For Life에는 재생 대기열, 반복 모드, 오디오 믹서 옵션, 그리고 더 몰입감 있는 감상을 위한 비주얼라이저 모드도 포함되어 있습니다.

            온라인 음악 탐색, 라디오 스트리밍, 편안한 배경음 재생, 저장한 목록 관리까지, Music For Life는 필요한 기능을 한곳에 모아 줍니다.
            """
        ).strip(),
    },
    "nl": {
        "short": "Muziek, radio, rustgevende geluiden en offline-playlists in een app.",
        "full": dedent(
            """
            Music For Life brengt online muziek, internetradio, ontspannende geluidscollecties en je eigen offline bibliotheek samen in een gebruiksvriendelijke speler.

            Ontdek muziek op taal, artiest, album, genre of jaar. Zoek snel naar nummers, open songlinks, deel favorieten en bekijk songteksten wanneer die beschikbaar zijn. Je kunt ook nummers, radiozenders en geluidsplaylists opslaan in je persoonlijke offline verzameling om ze later eenvoudig opnieuw af te spelen.

            Importeer muziek vanaf je apparaat of een volledige map, organiseer inhoud in playlists en maak een back-up van je bibliotheek met JSON-export en -import. Music For Life biedt daarnaast een afspeelwachtrij, herhaalstanden, audio-mixeropties en een visualizer voor een meeslependere luisterervaring.

            Of je nu online nieuwe muziek wilt ontdekken, radio wilt streamen, rustgevende achtergrondgeluiden wilt afspelen of je opgeslagen lijsten wilt beheren, Music For Life houdt alles op een plek.
            """
        ).strip(),
    },
    "pl": {
        "short": "Muzyka, radio, dzwieki relaksacyjne i playlisty offline w jednej aplikacji.",
        "full": dedent(
            """
            Music For Life laczy utwory online, radio internetowe, relaksujace kolekcje dzwiekow oraz Twoja wlasna biblioteke offline w jednym latwym w obsludze odtwarzaczu.

            Odkrywaj muzyke wedlug jezyka, artysty, albumu, gatunku lub roku. Szybko wyszukuj utwory, otwieraj linki do piosenek, udostepniaj ulubione i wyswietlaj teksty, gdy sa dostepne. Mozesz tez zapisywac piosenki, stacje radiowe i playlisty dzwiekowe w swojej kolekcji offline, aby latwo do nich wracac.

            Importuj muzyke z urzadzenia lub calego folderu, organizuj zawartosc w playlisty i tworz kopie zapasowe biblioteki za pomoca eksportu oraz importu JSON. Music For Life oferuje rowniez kolejke odtwarzania, tryby petli, opcje miksera audio i tryb wizualizera dla bardziej wciagajacego sluchania.

            Niezaleznie od tego, czy chcesz odkrywac nowa muzyke online, sluchac radia, odtwarzac uspokajajace dzwieki w tle czy zarzadzac zapisanymi listami, Music For Life trzyma wszystko w jednym miejscu.
            """
        ).strip(),
    },
    "pt": {
        "short": "Musica, radio, sons relaxantes e playlists offline em um so app.",
        "full": dedent(
            """
            Music For Life reune musicas online, radio pela internet, colecoes de sons relaxantes e sua propria biblioteca offline em um player facil de usar.

            Descubra musicas por idioma, artista, album, genero ou ano. Pesquise faixas rapidamente, abra links de musicas, compartilhe favoritas e veja letras quando estiverem disponiveis. Voce tambem pode salvar musicas, estacoes de radio e playlists de sons na sua colecao offline para ouvir novamente quando quiser.

            Importe musicas do seu dispositivo ou de uma pasta inteira, organize o conteudo em playlists e faca backup da biblioteca com ferramentas de exportacao e importacao JSON. Music For Life tambem inclui fila de reproducao, modos de repeticao, opcoes de mixer de audio e um modo visualizer para uma experiencia mais envolvente.

            Seja para descobrir novas musicas online, ouvir radio, reproduzir sons de fundo relaxantes ou gerenciar listas salvas, Music For Life mantem tudo em um so lugar.
            """
        ).strip(),
    },
    "ru": {
        "short": "Музыка, радио, расслабляющие звуки и офлайн-плейлисты в одном приложении.",
        "full": dedent(
            """
            Music For Life объединяет онлайн-песни, интернет-радио, расслабляющие звуковые подборки и вашу личную офлайн-библиотеку в одном удобном плеере.

            Открывайте музыку по языку, исполнителю, альбому, жанру или году. Быстро ищите треки, открывайте ссылки на песни, делитесь избранным и просматривайте тексты, если они доступны. Вы также можете сохранять песни, радиостанции и звуковые подборки в свою офлайн-коллекцию, чтобы легко возвращаться к ним позже.

            Импортируйте музыку с устройства или из целой папки, организуйте контент по плейлистам и создавайте резервные копии библиотеки с помощью экспорта и импорта JSON. Music For Life также предлагает очередь воспроизведения, режимы повтора, настройки аудиомикшера и визуализатор для более яркого прослушивания.

            Хотите ли вы находить новую музыку онлайн, слушать радио, включать спокойные фоновые звуки или управлять сохраненными списками, Music For Life держит все в одном месте.
            """
        ).strip(),
    },
    "th": {
        "short": "ฟังเพลง วิทยุ เสียงผ่อนคลาย และเพลย์ลิสต์ออฟไลน์ในแอปเดียว",
        "full": dedent(
            """
            Music For Life รวมเพลงออนไลน์ วิทยุอินเทอร์เน็ต ชุดเสียงผ่อนคลาย และคลังเพลงออฟไลน์ส่วนตัวของคุณไว้ในเครื่องเล่นเดียวที่ใช้งานง่าย

            ค้นหาเพลงตามภาษา ศิลปิน อัลบั้ม แนวเพลง หรือปีได้อย่างสะดวก ค้นหาเพลงได้รวดเร็ว เปิดลิงก์เพลง แชร์เพลงโปรด และดูเนื้อเพลงเมื่อมีให้ใช้งาน คุณยังสามารถบันทึกเพลง สถานีวิทยุ และเพลย์ลิสต์เสียงไว้ในคอลเลกชันออฟไลน์ส่วนตัวเพื่อกลับมาฟังได้ทุกเมื่อ

            นำเข้าเพลงจากอุปกรณ์หรือทั้งโฟลเดอร์ จัดระเบียบเนื้อหาเป็นเพลย์ลิสต์ และสำรองข้อมูลคลังเพลงด้วยการส่งออกและนำเข้าแบบ JSON ได้ Music For Life ยังมีคิวการเล่น โหมดวนซ้ำ ตัวปรับแต่งเสียง และโหมดภาพแสดงจังหวะเพื่อประสบการณ์การฟังที่สนุกยิ่งขึ้น

            ไม่ว่าคุณจะอยากค้นหาเพลงออนไลน์ ฟังวิทยุ เปิดเสียงพื้นหลังเพื่อการพักผ่อน หรือจัดการเพลย์ลิสต์ที่บันทึกไว้ Music For Life ก็รวบรวมทุกอย่างไว้ในที่เดียว
            """
        ).strip(),
    },
    "tr": {
        "short": "Muzik, radyo, rahatlatici sesler ve cevrimdisi listeler tek uygulamada.",
        "full": dedent(
            """
            Music For Life, cevrimici sarkilari, internet radyosunu, rahatlatici ses koleksiyonlarini ve kendi cevrimdisi kutuphanenizi kullanimi kolay tek bir oynaticida bir araya getirir.

            Dile, sanatciya, albume, ture veya yila gore muzik kesfedin. Sarkilari hizli arayin, baglantilari acin, favorilerinizi paylasin ve mevcutsa sarki sozlerini goruntuleyin. Sarkilari, radyo istasyonlarini ve ses listelerini kisisel cevrimdisi koleksiyonunuza kaydederek daha sonra kolayca tekrar dinleyebilirsiniz.

            Muzigi cihazinizdan veya tum bir klasorden ice aktarabilir, icerigi oynatma listelerinde duzenleyebilir ve kutuphanenizi JSON disa aktarma ve ice aktarma araclariyla yedekleyebilirsiniz. Music For Life ayrica oynatma kuyrugu, tekrar modlari, ses mikseri secenekleri ve daha surukleyici bir dinleme deneyimi icin bir gorsellestirici modu sunar.

            Ister yeni muzikler kesfetmek, ister radyo dinlemek, ister rahatlatici arka plan sesleri calmak ya da kaydedilen listeleri yonetmek isteyin, Music For Life her seyi tek bir yerde toplar.
            """
        ).strip(),
    },
    "zh-CN": {
        "short": "在一个应用中畅听音乐、电台、环境音与离线歌单。",
        "full": dedent(
            """
            Music For Life 将在线歌曲、网络电台、放松环境音以及你的个人离线音乐库整合到一个易于使用的播放器中。

            你可以按语言、艺人、专辑、流派或年份发现音乐，快速搜索歌曲、打开歌曲链接、分享收藏，并在有内容时查看歌词。你还可以把歌曲、电台和环境音播放列表保存到个人离线收藏中，方便以后随时继续收听。

            应用支持从设备或整个文件夹导入音乐，将内容整理为播放列表，并通过 JSON 导出与导入工具备份音乐库。Music For Life 还提供播放队列、循环模式、音频混音设置，以及更具沉浸感的可视化模式。

            无论你想在线发现新音乐、收听电台、播放舒缓背景音，还是管理已保存的播放列表，Music For Life 都能把这些功能集中在同一个地方。
            """
        ).strip(),
    },
}


def rgb(hex_color: str) -> tuple[int, int, int]:
    hex_color = hex_color.lstrip("#")
    return tuple(int(hex_color[i : i + 2], 16) for i in (0, 2, 4))


PALETTE = {
    "orange": rgb("#ff9409"),
    "orange_soft": rgb("#ffb347"),
    "red": rgb("#ee2c3c"),
    "wine": rgb("#9f1d35"),
    "cream": rgb("#ffd6c9"),
    "rose": rgb("#ffc0b5"),
    "shadow": rgb("#6c1023"),
    "white": rgb("#ffffff"),
}


def ensure_font(path: str, size: int) -> ImageFont.FreeTypeFont | ImageFont.ImageFont:
    try:
        return ImageFont.truetype(path, size=size)
    except OSError:
        return ImageFont.load_default()


TITLE_FONT = "/System/Library/Fonts/Avenir Next Condensed.ttc"
BODY_FONT = "/System/Library/Fonts/Avenir Next.ttc"
FALLBACK_UNICODE_FONT = "/Library/Fonts/Arial Unicode.ttf"


def lerp(a: float, b: float, t: float) -> float:
    return a + (b - a) * t


def lerp_color(c1: tuple[int, int, int], c2: tuple[int, int, int], t: float) -> tuple[int, int, int]:
    return tuple(int(lerp(c1[i], c2[i], t)) for i in range(3))


def make_gradient(width: int, height: int, tl: tuple[int, int, int], br: tuple[int, int, int]) -> Image.Image:
    img = Image.new("RGBA", (width, height))
    px = img.load()
    max_mix = max(width + height - 2, 1)
    for y in range(height):
        for x in range(width):
            mix = (x + y) / max_mix
            r, g, b = lerp_color(tl, br, mix)
            px[x, y] = (r, g, b, 255)
    return img


def blur_glow(size: tuple[int, int], bbox: tuple[int, int, int, int], color: tuple[int, int, int], alpha: int, blur: int) -> Image.Image:
    layer = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(layer)
    draw.ellipse(bbox, fill=(*color, alpha))
    return layer.filter(ImageFilter.GaussianBlur(blur))


def draw_icon_surface(size: int = 512) -> Image.Image:
    img = make_gradient(size, size, PALETTE["orange"], PALETTE["red"])
    draw = ImageDraw.Draw(img)
    inset = int(size * 0.03)
    radius = int(size * 0.16)

    mask = Image.new("L", (size, size), 0)
    ImageDraw.Draw(mask).rounded_rectangle((0, 0, size - 1, size - 1), radius=radius, fill=255)
    rounded = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    rounded.paste(img, (0, 0), mask)
    img = rounded

    img.alpha_composite(blur_glow((size, size), (50, 40, size - 110, size - 90), PALETTE["orange_soft"], 70, 32))
    img.alpha_composite(blur_glow((size, size), (size // 2 - 90, size // 2 - 70, size // 2 + 90, size // 2 + 110), PALETTE["cream"], 55, 20))

    draw = ImageDraw.Draw(img)
    draw.rounded_rectangle((inset, inset, size - inset, size - inset), radius=radius - inset, outline=(255, 196, 170, 80), width=6)

    shadow_layer = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    shadow = ImageDraw.Draw(shadow_layer)
    shadow.arc((112, 95, 400, 335), start=188, end=352, fill=(*PALETTE["shadow"], 130), width=32)
    shadow.rounded_rectangle((102, 242, 176, 394), radius=34, fill=(*PALETTE["shadow"], 120))
    shadow.rounded_rectangle((336, 242, 410, 394), radius=34, fill=(*PALETTE["shadow"], 120))
    img.alpha_composite(shadow_layer.filter(ImageFilter.GaussianBlur(10)))

    draw.arc((112, 95, 400, 335), start=188, end=352, fill=(*PALETTE["cream"], 245), width=34)
    draw.arc((126, 110, 386, 322), start=188, end=352, fill=(*PALETTE["rose"], 180), width=8)
    draw.rounded_rectangle((100, 236, 180, 402), radius=40, fill=(*PALETTE["rose"], 235))
    draw.rounded_rectangle((332, 236, 412, 402), radius=40, fill=(*PALETTE["rose"], 235))
    draw.rounded_rectangle((118, 268, 176, 382), radius=28, fill=(*PALETTE["wine"], 230))
    draw.rounded_rectangle((336, 268, 394, 382), radius=28, fill=(*PALETTE["wine"], 230))
    draw.ellipse((240, 185, 328, 273), fill=(255, 150, 120, 185))
    draw.ellipse((124, 236, 162, 274), fill=(255, 233, 225, 85))
    draw.ellipse((348, 224, 386, 262), fill=(255, 233, 225, 85))

    return img


def wrap_text(draw: ImageDraw.ImageDraw, text: str, font, max_width: int) -> list[str]:
    words = text.split()
    lines: list[str] = []
    current = ""
    for word in words:
        candidate = word if not current else f"{current} {word}"
        width = draw.textbbox((0, 0), candidate, font=font)[2]
        if width <= max_width or not current:
            current = candidate
        else:
            lines.append(current)
            current = word
    if current:
        lines.append(current)
    return lines


def draw_feature_graphic() -> Image.Image:
    width, height = 1024, 500
    base = make_gradient(width, height, rgb("#ff8b0d"), rgb("#9e1738"))
    base.alpha_composite(blur_glow((width, height), (40, 30, 460, 430), PALETTE["orange_soft"], 95, 48))
    base.alpha_composite(blur_glow((width, height), (420, -40, 1060, 520), PALETTE["cream"], 55, 60))

    draw = ImageDraw.Draw(base)
    draw.rounded_rectangle((40, 40, width - 40, height - 40), radius=42, outline=(255, 228, 210, 48), width=3)

    icon_shadow = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    ImageDraw.Draw(icon_shadow).rounded_rectangle((96, 90, 416, 410), radius=54, fill=(94, 18, 35, 95))
    base.alpha_composite(icon_shadow.filter(ImageFilter.GaussianBlur(18)))

    icon = draw_icon_surface(320)
    base.alpha_composite(icon, (96, 90))

    title_font = ensure_font(TITLE_FONT, 58)
    subtitle_font = ensure_font(BODY_FONT, 28)
    chip_font = ensure_font(BODY_FONT, 24)

    title_x = 488
    draw.text((title_x, 122), "HEARTBEAT MUSIC", font=title_font, fill=(255, 248, 244, 255))

    subtitle = "Music, radio, offline playlists, lyrics, and visualizer."
    for i, line in enumerate(wrap_text(draw, subtitle, subtitle_font, 420)):
        draw.text((title_x, 210 + i * 36), line, font=subtitle_font, fill=(255, 233, 224, 235))

    chips = ["MUSIC", "RADIO", "OFFLINE", "LYRICS"]
    chip_y = 330
    chip_x = title_x
    for label in chips:
        text_box = draw.textbbox((0, 0), label, font=chip_font)
        chip_w = (text_box[2] - text_box[0]) + 34
        draw.rounded_rectangle((chip_x, chip_y, chip_x + chip_w, chip_y + 42), radius=21, fill=(130, 26, 52, 116), outline=(255, 236, 225, 135), width=2)
        draw.text((chip_x + 17, chip_y + 8), label, font=chip_font, fill=(255, 247, 242, 255))
        chip_x += chip_w + 12

    accent = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    acc_draw = ImageDraw.Draw(accent)
    acc_draw.line((486, 86, 486, 414), fill=(255, 232, 220, 55), width=3)
    base.alpha_composite(accent.filter(ImageFilter.GaussianBlur(1)))

    return base


def icon_svg() -> str:
    return dedent(
        f"""
        <svg xmlns="http://www.w3.org/2000/svg" width="512" height="512" viewBox="0 0 512 512">
          <defs>
            <linearGradient id="bg" x1="0%" y1="100%" x2="100%" y2="0%">
              <stop offset="0%" stop-color="#ff9409" />
              <stop offset="100%" stop-color="#ee2c3c" />
            </linearGradient>
            <filter id="softGlow" x="-20%" y="-20%" width="140%" height="140%">
              <feGaussianBlur stdDeviation="18" />
            </filter>
          </defs>
          <rect width="512" height="512" rx="82" fill="url(#bg)" />
          <rect x="14" y="14" width="484" height="484" rx="72" fill="none" stroke="rgba(255,230,220,0.35)" stroke-width="4" />
          <ellipse cx="212" cy="170" rx="130" ry="110" fill="#ffb347" opacity="0.18" filter="url(#softGlow)" />
          <ellipse cx="300" cy="264" rx="72" ry="72" fill="#ffd6c9" opacity="0.16" filter="url(#softGlow)" />
          <path d="M126 246 A130 130 0 0 1 386 246" fill="none" stroke="#ffd6c9" stroke-width="34" stroke-linecap="round" />
          <path d="M138 250 A118 118 0 0 1 374 250" fill="none" stroke="#ffc0b5" stroke-width="8" stroke-linecap="round" opacity="0.75" />
          <rect x="100" y="236" width="80" height="166" rx="40" fill="#ffc0b5" opacity="0.95" />
          <rect x="332" y="236" width="80" height="166" rx="40" fill="#ffc0b5" opacity="0.95" />
          <rect x="118" y="268" width="58" height="114" rx="28" fill="#9f1d35" opacity="0.92" />
          <rect x="336" y="268" width="58" height="114" rx="28" fill="#9f1d35" opacity="0.92" />
          <circle cx="256" cy="229" r="44" fill="#ff9f7c" opacity="0.72" />
        </svg>
        """
    ).strip() + "\n"


def feature_svg() -> str:
    return dedent(
        """
        <svg xmlns="http://www.w3.org/2000/svg" width="1024" height="500" viewBox="0 0 1024 500">
          <defs>
            <linearGradient id="bg" x1="0%" y1="100%" x2="100%" y2="0%">
              <stop offset="0%" stop-color="#ff8b0d" />
              <stop offset="100%" stop-color="#9e1738" />
            </linearGradient>
            <filter id="blur" x="-20%" y="-20%" width="140%" height="140%">
              <feGaussianBlur stdDeviation="28" />
            </filter>
          </defs>
          <rect width="1024" height="500" fill="url(#bg)" />
          <rect x="40" y="40" width="944" height="420" rx="42" fill="none" stroke="rgba(255,230,220,0.22)" stroke-width="3" />
          <ellipse cx="235" cy="180" rx="180" ry="130" fill="#ffb347" opacity="0.22" filter="url(#blur)" />
          <ellipse cx="760" cy="170" rx="260" ry="140" fill="#ffd6c9" opacity="0.12" filter="url(#blur)" />
          <g transform="translate(76,70) scale(0.703125)">
            <rect width="512" height="512" rx="82" fill="url(#bg)" />
            <rect x="14" y="14" width="484" height="484" rx="72" fill="none" stroke="rgba(255,230,220,0.35)" stroke-width="4" />
            <ellipse cx="212" cy="170" rx="130" ry="110" fill="#ffb347" opacity="0.18" filter="url(#blur)" />
            <ellipse cx="300" cy="264" rx="72" ry="72" fill="#ffd6c9" opacity="0.16" filter="url(#blur)" />
            <path d="M126 246 A130 130 0 0 1 386 246" fill="none" stroke="#ffd6c9" stroke-width="34" stroke-linecap="round" />
            <path d="M138 250 A118 118 0 0 1 374 250" fill="none" stroke="#ffc0b5" stroke-width="8" stroke-linecap="round" opacity="0.75" />
            <rect x="100" y="236" width="80" height="166" rx="40" fill="#ffc0b5" opacity="0.95" />
            <rect x="332" y="236" width="80" height="166" rx="40" fill="#ffc0b5" opacity="0.95" />
            <rect x="118" y="268" width="58" height="114" rx="28" fill="#9f1d35" opacity="0.92" />
            <rect x="336" y="268" width="58" height="114" rx="28" fill="#9f1d35" opacity="0.92" />
            <circle cx="256" cy="229" r="44" fill="#ff9f7c" opacity="0.72" />
          </g>
          <text x="488" y="160" fill="#fff7f2" font-size="58" font-family="Avenir Next Condensed, Arial, sans-serif" font-weight="700">HEARTBEAT MUSIC</text>
          <text x="500" y="220" fill="#ffe9e0" font-size="28" font-family="Avenir Next, Arial, sans-serif">
            <tspan x="500" dy="0">Music, radio, offline playlists,</tspan>
            <tspan x="500" dy="36">lyrics, and visualizer.</tspan>
          </text>
          <g font-family="Avenir Next, Arial, sans-serif" font-size="24" fill="#fffaf6">
            <rect x="500" y="322" width="112" height="42" rx="21" fill="rgba(255,245,238,0.22)" stroke="rgba(255,236,225,0.45)" stroke-width="2" />
            <text x="525" y="350">MUSIC</text>
            <rect x="624" y="322" width="110" height="42" rx="21" fill="rgba(255,245,238,0.22)" stroke="rgba(255,236,225,0.45)" stroke-width="2" />
            <text x="651" y="350">RADIO</text>
            <rect x="746" y="322" width="128" height="42" rx="21" fill="rgba(255,245,238,0.22)" stroke="rgba(255,236,225,0.45)" stroke-width="2" />
            <text x="770" y="350">OFFLINE</text>
            <rect x="886" y="322" width="106" height="42" rx="21" fill="rgba(255,245,238,0.22)" stroke="rgba(255,236,225,0.45)" stroke-width="2" />
            <text x="913" y="350">LYRICS</text>
          </g>
        </svg>
        """
    ).strip() + "\n"


def readme_name(locale: str) -> str:
    return "README.md" if locale == "en" else f"README.{locale}.md"


def render_listing(locale: str, short: str, full: str) -> str:
    short = short.replace("Music For Life", APP_NAME)
    full = full.replace("Music For Life", APP_NAME)
    if locale == "en":
        return (
            f"# {APP_NAME}\n\n"
            f"![{APP_NAME} Feature Graphic](infostore/playstore_feature_graphic.png)\n\n"
            f'<p align="center">\n'
            f'  <img src="infostore/playstore_app_icon.png" alt="{APP_NAME} App Icon" width="160" />\n'
            f"</p>\n\n"
            "## App Overview\n\n"
            f"App name: {APP_NAME}\n\n"
            f"Short description: {short}\n\n"
            "## Full Description\n\n"
            f"{full}\n"
        )
    return (
        f"# {APP_NAME}\n\n"
        "## Google Play Store Listing\n\n"
        f"App name: {APP_NAME}\n\n"
        f"Short description: {short}\n\n"
        "Full description:\n\n"
        f"{full}\n"
    )


def write_markdown_files() -> list[str]:
    written: list[str] = []
    INFOSTORE_DIR.mkdir(parents=True, exist_ok=True)
    locales = sorted(p.stem for p in LANG_DIR.glob("*.json"))
    for locale in locales:
        data = LISTING_DATA.get(locale, LISTING_DATA["en"])
        path = WORKSPACE_ROOT / "README.md" if locale == "en" else INFOSTORE_DIR / readme_name(locale)
        path.write_text(render_listing(locale, data["short"], data["full"]), encoding="utf-8")
        written.append(str(path.relative_to(WORKSPACE_ROOT)))
    return written


def write_image_files() -> list[str]:
    INFOSTORE_DIR.mkdir(parents=True, exist_ok=True)
    icon_png = INFOSTORE_DIR / "playstore_app_icon.png"
    feature_png = INFOSTORE_DIR / "playstore_feature_graphic.png"
    icon_svg_path = INFOSTORE_DIR / "playstore_app_icon.svg"
    feature_svg_path = INFOSTORE_DIR / "playstore_feature_graphic.svg"

    draw_icon_surface(512).save(icon_png)
    draw_feature_graphic().save(feature_png)
    icon_svg_path.write_text(icon_svg(), encoding="utf-8")
    feature_svg_path.write_text(feature_svg(), encoding="utf-8")

    return [
        str(icon_png.relative_to(WORKSPACE_ROOT)),
        str(feature_png.relative_to(WORKSPACE_ROOT)),
        str(icon_svg_path.relative_to(WORKSPACE_ROOT)),
        str(feature_svg_path.relative_to(WORKSPACE_ROOT)),
    ]


def main() -> None:
    md_files = write_markdown_files()
    image_files = write_image_files()
    print("Generated markdown files:")
    for name in md_files:
        print(f" - {name}")
    print("Generated image files:")
    for name in image_files:
        print(f" - {name}")


if __name__ == "__main__":
    main()
