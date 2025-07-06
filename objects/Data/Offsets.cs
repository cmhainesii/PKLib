using System.Reflection.Metadata;

public readonly struct Offsets
{
    public readonly ushort TrainerName { get; init; }
    public readonly ushort RivalName { get; init; }
    public readonly ushort Money { get; init; }
    public readonly ushort OwnedSeenSize { get; init; }
    public readonly ushort Owned { get; init; }
    public readonly ushort Seen { get; init; }
    public readonly ushort PartySize { get; init; }
    public readonly ushort BagSize { get; init; }
    public readonly ushort BallsPocketOffset { get; init; }
    public readonly ushort KeyItemsPocketOffset { get; init; }
    public readonly ushort TmPocketOffset { get; init; }
    public readonly ushort MainChecksumRegionStart { get; init; }
    public readonly ushort MainChecksumRegionEnd { get; init; }
    public readonly ushort MainChecksum { get; init; }
    public readonly ushort PartyNextPokemon { get; init; }
    public readonly ushort CurrentlySetBox { get; init; }
    public readonly ushort CurrentBoxDataBegin { get; init; }
    public readonly ushort NextBox { get; init; }
    public readonly ushort NextBoxPokemon { get; init; }
    public readonly ushort BoxLevel { get; init; }
    public readonly ushort BoxIv { get; init; }
    public readonly ushort BoxOtName { get; init; }
    public readonly ushort BoxNickname { get; init; }
    public readonly ushort BoxEv { get; init; }
    public readonly ushort Badges { get; init; }
    public readonly ushort TrainerNameSize { get; init; }
    public readonly ushort PartyOtId { get; init; }
    public readonly ushort Level { get; init; }
    public readonly ushort AttackDefense { get; init; }
    public readonly ushort SpeedSpecial { get; init; }
    public readonly ushort GenOneType1 { get; init; }
    public readonly ushort Stats { get; init; }
    public readonly ushort Ev { get; init; }
    public readonly ushort PartyOtName { get; init; }
    public readonly ushort PartyNickName { get; init; }
    public readonly ushort TrainerId { get; init; }
    public readonly ushort BankTwoBoxesStart { get; init; }
    public readonly ushort BankTwoBoxesEnd { get; init; }
    public readonly ushort BoxDataEnd { get; init; }
    public readonly ushort BoxChecksumsStart { get; init; }
    
    public static Offsets ForGeneration1() => new()
    {
        TrainerName = 0x2598,
        RivalName = 0x25F6,
        Money = 0x25F3,
        OwnedSeenSize = 0x13,
        Owned = 0x25A3,
        Seen = 0x25B6,
        PartySize = 0x2F2C,
        BagSize = 0x25C9,
        BallsPocketOffset = 0x00,
        KeyItemsPocketOffset = 0x00,
        TmPocketOffset = 0x00,
        MainChecksumRegionStart = 0x2598,
        MainChecksumRegionEnd = 0x3522,
        MainChecksum = 0x3523,
        PartyNextPokemon = 0x2C, // 44
        CurrentlySetBox = 0x284C,
        CurrentBoxDataBegin = 0x30C0,
        NextBox = 0x462,
        NextBoxPokemon = 0x21,
        BoxLevel = 0x03,
        BoxIv = 0x1B,
        BoxOtName = 0x2AA,
        BoxNickname = 0x386,
        BoxEv = 0x11,
        Badges = 0x2602,
        TrainerNameSize = 0x0B,
        PartyOtId = 0x0C,
        Level = 0x21,
        AttackDefense = 0x1B,
        SpeedSpecial = 0x1C,
        GenOneType1 = 0x05,
        Stats = 0x22,
        Ev = 0x11,
        PartyOtName = 0x110,
        PartyNickName = 0x152,
        TrainerId = 0x2605,
        BankTwoBoxesStart = 0x4000,
        BankTwoBoxesEnd = 0x5A4B,
        BoxDataEnd = 0x461,
        BoxChecksumsStart = 0x5A4C
    };

    public static Offsets ForCrystal() => new()
    {
        TrainerName = 0x200B,
        RivalName = 0x2021,
        Money = 0x23DC,
        OwnedSeenSize = 0x20, // 32 bits
        Owned = 0x2A27,
        Seen = 0x2A47,
        PartySize = 0x2865,
        BagSize = 0x2420,
        BallsPocketOffset = 0x2465,
        KeyItemsPocketOffset = 0x244A,
        TmPocketOffset = 0x23E7,
        MainChecksumRegionStart = 0x2009,
        MainChecksumRegionEnd = 0x2B82,
        MainChecksum = 0x2D0D,
        PartyNextPokemon = 48,
        CurrentlySetBox = 0x2700,
        CurrentBoxDataBegin = 0x2D10,
        NextBox = 0x450,
        NextBoxPokemon = 0x20,
        BoxLevel = 0x1F,
        BoxIv = 0x15,
        BoxOtName = 0x296,
        BoxNickname = 0x372,
        BoxEv = 0x0B,
        Badges = 0x23E5,
        TrainerNameSize = 0x0B,
        PartyOtId = 0x06,
        Level = 0x1F,
        AttackDefense = 0x15,
        SpeedSpecial = 0x16,
        Stats = 0x24,
        Ev = 0x0B,
        PartyOtName = 0x128,
        PartyNickName = 0x16A,
        TrainerId = 0x2009,
        BankTwoBoxesStart = 0x4000,
        BankTwoBoxesEnd = 0x5A4B,
        BoxDataEnd = 0x461,
        BoxChecksumsStart = 0x5A4C
    };

    public static Offsets ForGoldSilver() => new()
    {
        TrainerName = 0x200B,
        RivalName = 0x2021,
        Money = 0x23DB,
        OwnedSeenSize = 0x20, // 32 bits
        Owned = 0x2A4C,
        Seen = 0x2A6C,
        PartySize = 0x288A,
        BagSize = 0x241F,
        BallsPocketOffset = 0x2464,
        KeyItemsPocketOffset = 0x2449,
        TmPocketOffset = 0x23E6,
        MainChecksumRegionStart = 0x2009,
        MainChecksumRegionEnd = 0x2D68,
        MainChecksum = 0x2D69,
        PartyNextPokemon = 48,
        CurrentlySetBox = 0x2724,
        CurrentBoxDataBegin = 0x2D6C,
        NextBox = 0x450,
        NextBoxPokemon = 0x20,
        BoxLevel = 0x1F,
        BoxIv = 0x15,
        BoxOtName = 0x296,
        BoxNickname = 0x372,
        BoxEv = 0x0B,
        Badges = 0x23E4,
        TrainerNameSize = 0x0B,
        PartyOtId = 0x06,
        Level = 0x1F,
        AttackDefense = 0x15,
        SpeedSpecial = 0x16,
        Stats = 0x24,
        Ev = 0x0B,
        PartyOtName = 0x128,
        PartyNickName = 0x16A,
        TrainerId = 0x2009,
        BankTwoBoxesStart = 0x4000,
        BankTwoBoxesEnd = 0x5A4B,
        BoxDataEnd = 0x461,
        BoxChecksumsStart = 0x5A4C
    };

    // Convenience method for backwards compatibility
    public static Offsets ForGeneration(int generation, bool crystal = false)
    {
        return generation switch
        {
            1 => ForGeneration1(),
            2 when crystal => ForCrystal(),
            2 => ForGoldSilver(),
            _ => throw new ArgumentException($"Unsupported generation: {generation}")
        };
    }
}

