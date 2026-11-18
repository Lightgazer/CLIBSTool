public static partial class CodePage
{
    // Preserved German tables formerly selected by English.
    private static char[] GetGermanCodePage(int num, string name)
    {
        if (num == 0)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 1)
            num = 2457;
        if (num == 2)
            num = 2460;
        if (num == 4)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 6)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 11)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 13)
        {
            if (name == "mission.ar")
                num = 2261;
            if (name == "evitem.ar")
                num = 2449;
            if (name == "areanml.ar")
                return "ABCD\u3000EFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]ÄäÖöÜüß„”&".ToCharArray();
            if (name == "townarea.ar")
                return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”&".ToCharArray();
        }
        if (num == 14)
        {
            if (name == "sysfont.ar")
                num = 549;
            if (name == "namene.ar")
                num = 552;
            if (name == "helpmsg.ar")
                return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”&=".ToCharArray();
            if (name == "helpmsgb.ar")
                return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”&".ToCharArray();
            if (name == "smenui.ar")
                return "ABCD\u3000EFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]ÄäÖöÜüß„”&".ToCharArray();
        }
        if (num == 549)  //same as 14.sysfont
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”&".ToCharArray();
        if (num == 552)  //same as 14.namene
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”&".ToCharArray();
        if (num == 2256)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 2257)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 2261)  //same as 13.mission
            return "ABCD\u3000EFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]ÄäÖöÜüß„”&".ToCharArray();
        if (num == 2264)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”".ToCharArray();
        if (
            num == 2263 || num == 2265 || num == 2266 || num == 2267 || num == 2268 || num == 2269 ||
            num == 2270 || num == 2271 || num == 2272 || num == 2273 || num == 2274 || num == 2275 ||
            num == 2276 || num == 2277 || num == 2278 || num == 2279
        )
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”&".ToCharArray();
        if (num == 2284)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”&".ToCharArray();
        if (num == 2449)  // same as 13.evitem
            return "ABCD\u3000EFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]ÄäÖöÜüß„”&".ToCharArray();
        if (num == 2455)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 2456)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 2457)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 2458)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 2459)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 2460)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„”".ToCharArray();
        if (num == 2462)
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”".ToCharArray();
        return ("？あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわを" +
            "んがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽっゃゅょぁぃぅぇぉアイウ" +
            "エオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポッャュョァィゥェォヴゎヮ0123456789ABCDEFGHI" +
            "JKLMNOPQRSTUVWXYZー!?、。「」Σ'Д`∀ヾ゜дω・\u3000㌧㌦㌍㌢ゝ秒abcdefghijklmnopqrstuvwxyz冬燦峰【】\u3000ÄäÖöÜüß„”=最後力大陸役立思許様他" +
            "干渉不幸招旅裏切神竜王類次時代委定族同士長苦戦現状正使限誰管理恐未来無関係墓暴解放玉永劫守仰一与彼支配者意志約束…争考少違本質的好認飽欲望進化鍵運命死言生物殺食動作弱肉強新世界摂甘自然愛草木重野獣性温" +
            "心信導目覚結果出残捧民救術教危機迫念資格仲知問題男狂気悪北沼封印責任待会談終日事託受証拠聖賜芳女何申破滅勇剣授手伝壁絵右光魔道書左宝黄金輝甲冑身暗黒向説今前御僕協話聞助願邪帝国侵略背度訳虐分悔騎誇味悟" +
            "上子軍内弟伯爵通以組敵方専門判断流水元十承行汝加護失探継描姿遣奇跡下団傍有由誉傭用要踊覧率轄屯侶了領剰輪麗歴偉録綺東友奉武至高是非卿勧礼先娘嘆兵多攻込座亡努務数年名参隊取避得決臆病熟詔婦預昨姪塞隣親派" +
            "公二乱始半市太優勢追詰求策注甦宿復活阻止老師実初典記恋悲落涙晶再遺存反崖静殿耐援到着余送簡共音沙括篭城続顔態打開安姫葉故唇奪笑南外布陣程規疲弊中募義訓練届集奴入報告所属明確馬鹿猛潔諸相困戻必変郷妃在抱" +
            "山賊征陛挙急宰保対持謝父海畑起恥侯遠場骸深負噂鵜呑絶罠論妥当早治回各防衛尽腰降期忠層働席主家冒涜議足近満片付口久修休喜頼直都合床伏歳政応逃快迎拶殊勝候恩揺留暮荒恨難差伸小頭皆侮痛末挨島貴暇忙嫌伺引煩怯" +
            "君帰達減真似騒面倒昔際姐俺張忘郎透丈夫連適緒耳健村警嘘赤興賭慢己体緊感欠律敢肩並売災冗去準備獲飛巡枚雇棒文句屁第割仕墜狩盗辺横襲撃住寄愚痴黙若祈統制群過利読基将荷担険炎形誓夢白想振叶福台詞慰縁貰血祭汰" +
            "怪昼闘答案散歩冷憎消価値建築朝夕閣令従陥突訪段平皇器万敗成就造短別特窟圧盾焦采牢構囚稼忍踏捕可店兄漢胸呼閉怖脱全部屋扉借酬払渋商買輸具衆額茶党請企騙筋縄妹風吹根巣奥洞覆雲延魂慌腕増隷母紙姉乞離寂返番街" +
            "旗厳隠祖旦港息官巫石像札幼町巻馳居計遅宮偽頃貸酒溜歌指単迷晴喫情寛広完河柄労密戒裕渡充呪誤異四採()収益靴泉泣濡拾赦浅恵乗誘拐謀討伐駒只聡誠慕評処遇斎客線千謳斐例件搬傷婚甥位臣狐退叔患露筒抜置鉄脚哀容" +
            "弾課贄拷刑闇吟遊詩呵/序章整及砦隙狙弁疑衣悠独杖予堅嗚威和腹震箱抵歓麻占犬刻惨抗品逸囲秘薬飲糸火射観怒良頑英雄繰罪史古移経腐階級沿岸点臨操月寝洗熱屈牧川泳溺惑天原営室字学礎田舎憶夜斥装妙業鎧揮精鋭首香" +
            "豚尻叩速走発畜燃途端毒焼朗被害臭慎標歯車刃為糧讐癒虫捜索湾越肥沃域擁敷帯版図搾貧憤権鎮粛清希包影百裔済契服陰積飢肝漁穫杯揃鷹牙睨押接損敬徳勘玄常謎料刀儡傀汚掘亀砲技極縛稽油駄詮供鼻触灸検虎叱監視驚痩弓" +
            "声孫廃矢悼給笛橋副幾撤側美拭船互潮壊脅訴易把握辛晩惜鎖跳窮崩掌随杞憂詳寒唱裂掠土揉境素皮厚比懐柔孤妻逆銀革紛森拓唯善乏司総潜花賞条檻虜嘲楽斬吐燻交絞吊弄費蹄棄周逝諦鳥養才能躍戮逗換調朽呟倉犠牲固鍛殻餞" +
            "狭毎埋折厄介表勲貨執赴酷狡猾税法蔵慣芸量泡紹泊滞納幽巧祥吸局因曲軽蔑貫蓄細工酔盟秩蜂僧猊嬢亭個卑劣遭育空角捉潰還蹴駆奸智購殲衝叡眼激瞬誕凄瞳拍慮咲庫波蘇葬湧召喚匹胃農選獄膨覇墟林狼択編画替示設範展停烏" +
            "穏齢這凶僻蛇療隔施慈青習冶砂丘等徒飼豊谷逐普尊員徴複雑傾扱懸勉羽匂傲穴朴仁斉栄験輩兼材骨双斧富即鮮ヶ賢芽節辞穢侍種堕蹂卵網列浄称粗低衰符試粉嬉攫挑財館控砕針米曇製康嫡障顧戯拒否如虚嫁腑紅染三唸販喉汁提" +
            "督稀遂灯況猶昏尋褒裁剥胆犯除袋浮里屠紋艦獅凡躾匿弔漠偵察倍軟禁貶児苛職庇拉致涯飾槍詫遥識岩旧后星髪冥祝駐功併両洲距弩挟超伍猟掛路抑拝混浪巨薄幻肌暑妾毛乳産痕忌迂却愉檄翼携般釈浴凌勤症厩巷雷講模投耗環勅" +
            "仇殴尚坊五算添醒托漆暖含舞遮沸号灰式秋儀恒歪社枢畏冴躙喧嘩貯仔殖貌献牛象魚那憧銘演沈語岬頂戴懲飯珍頻繁映煮埒悩盛織慨脈省埃傘八州隅筆妬峠嵐秀憐償錆辱汗謹系緯屍沢仮欺順絡愕濃徐脳績銅緑更底岳匠鳴罰七羊脂" +
            "婿賎励景央署壇繋緩宣睦恰隻締色豆寿縮蓮棲蛮氏賛効塔掃概唄勾寸鋒泥廷錯転叫微曽醜煉妨偶掟兆壮午凱儲痣槌烈疾徹俊敏瀕拡庶湖刺渾刹粋贅崇唐惚餓沖没怠看板鏡魅霧鬼璧劇凛培蒙昧藉悶蠢怨霊瘴蚊棺桶華雫雨逞禄抹寧贈" +
            "脹羨撒○×△□鋼登+餌飴釣鞭姑楯科諌翻擬簒迅掲瀬堂俗\"舌噛侭頬磨摘祟隼照酌~肖遍猫楚眉盲免雪翌帳茨膚競嬌丸爛漫畔宗院博冠姻純渇鈴癖凧丁浸貢賀豪罵宴鈍乙陽昇芯便戸源幕禍排促滴創郊玩堵宅房改些澄航摯－喪Ⅰ" +
            "ⅡⅢⅣⅤⅥⅦⅧⅨⅩ諾僅補ゐゑヰヱヵ熊喋錠鼓架※*既電♪削蒼氷嗜卓渓咄嗟#$&湿阿呆枠此惹幹拘.催擲依暦粒栗較韋睡揚詠疫蔓区彷徨遙砥銑矛診尾蝉煽駿♂♀爪拳茂池六麓,:;¨^￣＿―-\\∥|‥‘’””〔〕[]" +
            "{}〈〉《》『』±÷=≠<>≦≧%§☆★●◎◇◆■▲▽▼→←↑↓АВГΔЕΖНΘΙКΛМΝΞОПРТΥФЧΨΩαβγδεζηθικλμνξοπρστυφχψ垣九岐寵球研樹蒸壺弦謁校賓園剤燕叙週＠＠辿維胎" +
            "瓦篤旨盆栽麦響脆箇項暫痺汎型竿壷噴凍蛾鱗耕亜鉛鉱錬隕譲頓憩醇克蕩沌馴虔徘徊宛査井汲碑貿綻推奨郭套灼載堪梁披幅贔屓嶺穀菜袈裟挽晒秤殉斃儚彫菩尖碧酵弐衡融疎哨薔薇縫陶鴨蟹淡酸爽薫庭炒蓋鶏兎茸羅貝柱苺季蜜塊鳳" +
            "凰髭硬粘幣溶橙屑牡猪鳩煎植鯨靱塵曜銛棍但妖嚇円盤捻銭淵煙妄胡述婆坑柵帆枯伴撹据眩濯慧臓雰奏桟医奮竹獰颯傑漏則塗珠蟲府吉趣廠洩諜捲聴罷袖循棚吏紳机絨毯磁瓶懺贖燈邸胞彩液琥珀享啓伊綴貼逢穿孔豹佐鉱帽絆膏扮潤" +
            "咳竪琴祓訛筈咬綿賦乾喘宜峙譜溢憬債童芝嫉拗矜囁垢雅苗篇猜毅囮陪卒暁扇站蒐絹蚤奔眺賃樽詣掴審懇畳藁膠測牽憑蹟逮楔巾涼薦械霞串漂塩頁瞭詐碍奢籍雛晰拵藪鞍牝僚究冊媚賑虹恭蒔鋤搭憲刈裾捌蝕鞘倹泰爺兜呈＠＠＠＠＠" +
            "＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠＠" +
            "＠＠＠＠＠＠＠＠＠").ToCharArray();    //8760 - space
    }
}
