namespace Bloxstrap.Enums
{
    public enum FontType
    {
        [EnumSort(Order = 1)]
        [EnumName(FromTranslation = "Common.Default")]
        Default,

        [EnumSort(Order = 2)]
        [EnumName(StaticName = "Noto Sans Thai")]
        NotoSansThai,

        [EnumSort(Order = 3)]
        [EnumName(StaticName = "Rubik")]
        Rubik,

        [EnumSort(Order = 4)]
        [EnumName(StaticName = "Accanthis")]
        Accanthis,

        [EnumSort(Order = 5)]
        [EnumName(StaticName = "Arial Bold")]
        ArialBold,

        [EnumSort(Order = 6)]
        [EnumName(StaticName = "Comic Sans")]
        ComicSans,

        [EnumSort(Order = 7)]
        [EnumName(StaticName = "Gotham")]
        Gotham,

        [EnumSort(Order = 8)]
        [EnumName(StaticName = "Gotham Bold")]
        GothamBold,

        [EnumSort(Order = 9)]
        [EnumName(StaticName = "Legacy Arial")]
        LegacyArial,

        [EnumSort(Order = 10)]
        [EnumName(StaticName = "Roboto")]
        Roboto,

        [EnumSort(Order = 11)]
        [EnumName(StaticName = "Roboto Mono")]
        RobotoMono,

        [EnumSort(Order = 12)]
        [EnumName(StaticName = "Source Sans Pro")]
        SourceSansPro,

        [EnumSort(Order = 13)]
        [EnumName(StaticName = "Arial")]
        Arial,

        [EnumSort(Order = 999)]
        [EnumName(FromTranslation = "Common.Custom")]
        Custom
    }
}