using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.XPath;
using PKLib;
namespace PKLib
{
    public class GameData
    {
        // Constants - Hex Data Offset Locations
        private const ushort BOX_SIZE_TO_FIRST_POKEMON_OFFSET = 0x16;
        private const ushort BOX_ONE_BEGIN = 0x4000;
        private const ushort BOX_SEVEN_BEGIN = 0x6000;

        private const ushort OT_NICK_NEXT_NAME_OFFSET = 0x0B;
        private const ushort PARTY_SIZE_TO_FIRST_POKEMON_OFFSET = 0x08;
        private readonly static ushort[] genderAndShadowOffsetCrystal = { 0x3E3D, 0x206A };
        private const int boxItemsSizeOffset = 0x27E6;
        private const int boxFirstItemOffset = boxItemsSizeOffset + 0x01;


        // Fields
        private byte[] _fileData;
        public string FileName { get; set; }
        public ushort Generation { get; set; }
        public bool IsCrystal { get; set; }
        public Party PartyPokemon { get; set;  }
        public PokemonPC PcPokemon { get; set;  }
        public Bag Items { get; set; }
        public ItemBox BoxItems { get; }
        public string TimeResetPassword { get; set; }
        public Trainer Trainer { get; set; }

        private readonly Offsets _offsets;
        internal ItemData itemData;
        internal PokemonData pokemonData;



        // Constructor
        public GameData(string fileName)
        {
            _fileData = File.ReadAllBytes(fileName);
            this.FileName = fileName;
            this.Generation = determineGeneration();
            _offsets = Offsets.ForGeneration(Generation, IsCrystal);
            itemData = new ItemData(Generation);
            pokemonData = new PokemonData(Generation);
            TimeResetPassword = string.Empty;

            if (Generation == 2)
            {
                TimeResetPassword = PokemonUtil.GCSTimeResetPassword(GetTrainerName(), int.Parse(GetTrainerID()), (int)(GetMoney()));
            }




            PartyPokemon = new Party(GetPartyPokemon());
            PcPokemon = new PokemonPC(this);
            if (Generation == 1)
            {
                Items = new Bag(GetBagItems(_offsets.BagSize, 20));
            }
            else
            {
                Items = new Bag(GetBagItems(_offsets.BagSize, 20),
                GetBagItems(_offsets.BallsPocketOffset, 12),
                GetBagItems(_offsets.KeyItemsPocketOffset, 26, false),
                GetTMPocketItems(_offsets.TmPocketOffset));
            }

            BoxItems = new ItemBox(GetBoxItems());

            Trainer = new Trainer
            {
                Name = GetTrainerName(),
                Id = GetTrainerID(),
                RivalName = GetRivalName(),
                NumberBadges = GetBadges().GetNumBadges(),
                Money = GetMoney()

            };


        }

        public GameData(byte[] fileData)
        {
            this._fileData = fileData;
            this.FileName = "save.sav";
            this.Generation = determineGeneration();
            _offsets = Offsets.ForGeneration(Generation, IsCrystal);
            itemData = new ItemData(Generation);
            pokemonData = new PokemonData(Generation);
            TimeResetPassword = string.Empty;

            if (Generation == 2)
            {
                TimeResetPassword = PokemonUtil.GCSTimeResetPassword(GetTrainerName(), int.Parse(GetTrainerID()), (int)(GetMoney()));
            }



            PartyPokemon = new Party(GetPartyPokemon());
            PcPokemon = new PokemonPC(this);
            if (Generation == 1)
            {
                Items = new Bag(GetBagItems(_offsets.BagSize, 20));
            }
            else
            {
                Items = new Bag(GetBagItems(_offsets.BagSize, 20),
                GetBagItems(_offsets.BallsPocketOffset, 12),
                GetBagItems(_offsets.KeyItemsPocketOffset, 26, false),
                GetTMPocketItems(_offsets.TmPocketOffset));
            }

            BoxItems = new ItemBox(GetBoxItems());

            Trainer = new Trainer
            {
                Name = GetTrainerName(),
                Id = GetTrainerID(),
                RivalName = GetRivalName(),
                NumberBadges = GetBadges().GetNumBadges(),
                Money = GetMoney()

            };


        }

        // Returns 1 if generation 1, 2 if generation 2.


        // Function to insert data to the game save file
        // newData = byte array of data to insert
        // startOffset = where to begin insertion
        public void PatchHexBytes(byte[] newData, int startOffset)
        {
            if (newData.Length <= 0)
            {
                Console.WriteLine("Error: New data size is zero.");
                return;
            }
            for (int i = 0; i < newData.Length; ++i)
            {
                _fileData[startOffset + i] = newData[i];
            }
        }

        internal void SetGender(byte gender)
        {
            if (!IsCrystal)
            {
                Console.WriteLine("Error, changing gender is only supported for Pokemon Crystal saves. Aborting.");
                return;
            }
            if (gender > 1 || gender < 0)
            {
                Console.WriteLine("Error: Gender must be set to either 0 (male) or 1 (female). Aborting");
                return;
            }

            PatchHexByte(gender, genderAndShadowOffsetCrystal[0]);
            PatchHexByte(gender, genderAndShadowOffsetCrystal[1]);
        }

        private ushort determineGeneration()
        {
            ReadOnlySpan<byte> save = GetSaveData();
            bool valid = false;
            valid = ValidateList(save, 0x2F2C, 6) && ValidateList(save, 0x30C0, 20);
            if (valid)
            {
                this.Generation = (ushort)1;
                this.IsCrystal = false;
                return this.Generation;
            }


            valid = ValidateList(save, 0x2ED5, 6) && ValidateList(save, 0x302D, 30);
            if (valid)
            {
                this.Generation = (ushort)1;
                this.IsCrystal = false;
                return this.Generation;
            }

            valid = ValidateList(save, 0x288A, 6) && ValidateList(save, 0x2D6C, 20);
            if (valid)
            {
                this.Generation = (ushort)2;
                this.IsCrystal = false;
                return this.Generation;
            }

            valid = ValidateList(save, 0x2865, 6) && ValidateList(save, 0x2D10, 20);
            if (valid)
            {
                this.Generation = (ushort)2;
                this.IsCrystal = true;
                return this.Generation;
            }

            valid = ValidateList(save, 0x283E, 6) && ValidateList(save, 0x2D10, 30);
            if (valid)
            {
                this.Generation = (ushort)2;
                this.IsCrystal = false;
                return this.Generation;
            }
            valid = ValidateList(save, 0x281A, 6) && ValidateList(save, 0x2D10, 30);
            if (valid)
            {
                this.Generation = (ushort)2;
                this.IsCrystal = true;
                return this.Generation;
            }

            return 0;
        }

        private bool ValidateList(ReadOnlySpan<byte> data, int offset, int maxEntries)
        {
            byte listLength = data[offset];
            return listLength <= maxEntries && data[offset + listLength + 1] == 0xFF;
        }


        public ReadOnlySpan<byte> GetSaveData()
        {
            return _fileData;
        }
        // Insert a single byte of data at a given offset
        public void PatchHexByte(byte newData, int offset)
        {
            _fileData[offset] = newData;
        }

        public void UpdateBoxChecksums()
        {
            if (Generation != 1)
            {
                return;
            }

            byte[] checksumsBank1 = new byte[6];
            // int[] checksumsBank2 = new int[6];
            int currentBoxStart = _offsets.BankTwoBoxesStart;

            for (int i = 0; i < 6; ++i)
            {
                int checksum = CalculateChecksum(currentBoxStart, currentBoxStart + _offsets.BoxDataEnd);
                byte lowByte = (byte)(checksum & 0xFF);
                checksumsBank1[i] = lowByte;
                currentBoxStart += _offsets.NextBox;
            }

            for (int i = 1; i <= 6; ++i)
            {
                Console.WriteLine($"Box {i} Checksum: {checksumsBank1[i - 1]:X2}");
            }

            PatchHexBytes(checksumsBank1, _offsets.BoxChecksumsStart + 1);

            int bankTwoWholeChecksum = CalculateChecksum(_offsets.BankTwoBoxesStart, _offsets.BankTwoBoxesEnd);
            byte bankTwoLowByte = (byte)(bankTwoWholeChecksum & 0xFF);
            Console.WriteLine($"Bank 2 Checksum: {bankTwoLowByte:X2}");
            PatchHexByte(bankTwoLowByte, _offsets.BoxChecksumsStart);





        }



        public int CalculateChecksum(int start, int end)
        {
            int sum = 0;
            Console.WriteLine($"Calculating checksum from {start:X} to {end:X}");
            for (int i = start; i <= end; i++)
            {
                sum += _fileData[i];
            }

            if (Generation == 1)
            {
                return ~sum & 0xFF;
            }
            else
            {
                sum = ((sum & 0xFF) << 8) | ((sum >> 8) & 0xFF); // Swap bytes for big-endian
                return sum & 0xFFFF;
            }
        }

        // Fetch a byte of data from a given offset
        public byte GetData(int offset)
        {
            if (offset < 0 || offset >= _fileData.Length)
            {
                throw new ArgumentException("Invalid offset provided. Aborting data retrevial.");
            }
            return _fileData[offset];
        }

        // Fetch a byte array from fileData between the start and end offset (inclusive).
        public byte[] GetData(int startOffset, int endOffset)
        {
            if (startOffset < 0 || endOffset < startOffset || endOffset >= _fileData.Length)
            {
                throw new ArgumentException("Invalid start or end offset.");
            }

            int length = endOffset - startOffset + 1;
            byte[] result = new byte[length];
            Array.Copy(_fileData, startOffset, result, 0, length);

            return result;
        }

        public int GetDataSize()
        {
            return _fileData.Length;
        }

        public void EmptyBag()
        {
            Items.ClearBag();

            byte[] bagClear = { 0x00, 0xFF };
            PatchHexBytes(bagClear, _offsets.BagSize);
        }

        public void ToFile()
        {
            UpdateBoxChecksums();
            UpdateMainChecksum(CalculateChecksum(
                _offsets.MainChecksumRegionStart,
                _offsets.MainChecksumRegionEnd
            ));

            File.WriteAllBytes(FileName, _fileData);
        }

        public string GetTrainerName()
        {
            return TextEncoding.GetEncodedText(this, _offsets.TrainerName, 0x50, _offsets.TrainerNameSize);
        }

        public string GetRivalName()
        {
            return TextEncoding.GetEncodedText(this, _offsets.RivalName, 0x50, _offsets.TrainerNameSize);
        }

        public void ChangeRivalName(string name)
        {
            if (name.Length > 7 || name.Length <= 0)
            {
                Console.WriteLine("Error: Name must be between 1 and 7 characters.");
                return;
            }

            byte[] encodedName = EncodeText(name, 0x50);
            if (encodedName.Length > _offsets.TrainerNameSize)
            {
                Console.WriteLine("Error: Encoded name text too long.");
                return;
            }
            PatchHexBytes(encodedName, _offsets.RivalName);
        }

        public void ChangePartyPokemonOtId(int newID)
        {
            int idOffset = 0x2605;

            byte[] newId = HexFunctions.ConvertIntToHexBytes(54321);

            for (int i = 0; i < newId.Length; ++i)
            {
                _fileData[idOffset + i] = newId[i];
            }

            int partyOffset = 0x2F2C;
            int firstPokemon = partyOffset + 0x08;
            int firstPokemonOtId = firstPokemon + _offsets.PartyOtId;

            Console.WriteLine("Total Pokemon in Party: " + _fileData[partyOffset]);
            int currentOffset = firstPokemonOtId;
            for (int j = 0; j < _fileData[partyOffset]; ++j)
            {
                PatchHexBytes(newId, currentOffset);
                currentOffset += 0x2C;
            }
        }

        public Badges GetBadges()
        {
            bool[] values = new bool[8];

            byte data = GetSaveData()[_offsets.Badges];

            for (byte index = 0; index < 8; ++index)
            {
                if (HexFunctions.BitIsSet(data, index))
                {
                    values[index] = true;
                }
            }

            Badges badges = new Badges(values);

            return badges;
        }


        private int DecodeBCD(byte bcdByte)
        {
            return (bcdByte >> 4) * 10 + (bcdByte & 0xF);
        }

        // Returns 32bit integer representing how much money the player has in decimal.
        public uint GetMoney()
        {
            byte[] money = GetData(_offsets.Money, _offsets.Money + 2);

            if (Generation == 1)
            {
                uint result = (uint)(DecodeBCD(money[0]) * 10000 +
                            DecodeBCD(money[1]) * 100 +
                            DecodeBCD(money[2]));
                return result;
            }
            else
            {

                // Combine bytes (big-endian)
                uint result = (uint)(money[2] | (money[1] << 8) | (money[0] << 16));
                return result;
            }
        }


        public ushort GetNumberOwned()
        {
            ushort sum = 0;
            for (int i = _offsets.Owned; i < _offsets.Owned + _offsets.OwnedSeenSize; ++i)
            {
                sum += getSumBits(_fileData[i]);
            }

            return sum;
        }

        public ushort GetNumberSeen()
        {
            ushort sum = 0;
            for (int i = _offsets.Seen; i < _offsets.Seen + _offsets.OwnedSeenSize; ++i)
            {
                sum += getSumBits(_fileData[i]);
            }

            return sum;
        }

        private ushort getSumBits(byte input)
        {
            ushort count = 0;

            for (int i = 0; i < 8; ++i)
            {
                if ((input & (1 << i)) != 0)
                {
                    count++;
                }
            }

            return count;
        }

        public ushort GetPartySize()
        {

            return (ushort)GetData(_offsets.PartySize);

        }

        internal ushort GetBoxSize(ushort boxNum)
        {
            if (GetData(_offsets.CurrentlySetBox) + 1 == boxNum)
            {
                return (ushort)GetData(_offsets.CurrentBoxDataBegin);

            }
            else
            {
                if (Generation == 1)
                {
                    if (boxNum < 7)
                    {
                        return (ushort)GetData(BOX_ONE_BEGIN + ((boxNum - 1) * _offsets.NextBox));
                    }
                    else
                    {
                        return (ushort)GetData(BOX_SEVEN_BEGIN + ((boxNum - 7) * _offsets.NextBox));
                    }
                }
                else
                {
                    return GetData(BOX_ONE_BEGIN + ((boxNum - 1) * _offsets.NextBox));
                }
            }
        }

        private string GetPartyPokemonName(ushort num)
        {
            if (num <= 0 || num >= 7)
            {
                throw new ArgumentException("Invalid party index! Must be 1 - 6");
            }

            if (num > GetPartySize())
            {
                throw new ArgumentException($"Invalid party index. Exceeds party size of {GetPartySize()}.");
            }
            try
            {
                string result = pokemonData.GetPokemonName(GetData(_offsets.PartySize + num));
                return result;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "Undefined";
            }
        }


        internal List<Pokemon> GetBoxPokemon(ushort boxNumber)
        {
            List<Pokemon> boxPokemon = new List<Pokemon>();
            if (boxNumber < 1 || boxNumber > 12)
            {
                throw new ArgumentException("Error: Invalid box number. Aborting.");
            }

            Pokemon current;
            string name;
            ushort level;
            byte ad, ss;
            ushort attack, defense, speed, special, hp;
            IV ivs;
            EVs evs;
            int currentBoxOffset;
            int currentPokemonOffset;
            int otNameOffset;
            int nickNameOffset;
            int friendshipValue;
            string otName;
            string nickname;
            string[] types = new string[2];
            byte[] hexIn = new byte[2];
            ushort cursor;
            if (GetData(_offsets.CurrentlySetBox) + 1 == boxNumber)
            {
                currentBoxOffset = _offsets.CurrentBoxDataBegin;
                currentPokemonOffset = currentBoxOffset + BOX_SIZE_TO_FIRST_POKEMON_OFFSET;
            }
            else
            {
                if (Generation == 1)
                {
                    if (boxNumber < 7)
                    {
                        currentBoxOffset = BOX_ONE_BEGIN + ((boxNumber - 1) * _offsets.NextBox);
                    }
                    else
                    {
                        currentBoxOffset = BOX_SEVEN_BEGIN + ((boxNumber - 7) * _offsets.NextBox);
                    }
                }
                else
                {
                    currentBoxOffset = BOX_ONE_BEGIN + ((boxNumber - 1) * _offsets.NextBox);
                }

                currentPokemonOffset = currentBoxOffset + BOX_SIZE_TO_FIRST_POKEMON_OFFSET;

            }
            int boxSize = GetBoxSize(boxNumber);

            if (boxSize < 1 || boxSize == 255)
            {
                return boxPokemon;
            }


            for (ushort i = 1; i <= boxSize; ++i)
            {


                name = pokemonData.GetPokemonName(GetData(currentPokemonOffset));
                level = (ushort)GetData(currentPokemonOffset + _offsets.BoxLevel);
                ad = GetData(currentPokemonOffset + _offsets.BoxIv);
                ss = GetData(currentPokemonOffset + _offsets.BoxIv + 1);
                attack = (ushort)(ad >> 4);
                defense = (ushort)(ad & 0x0F);
                speed = (ushort)(ss >> 4);
                special = (ushort)(ss & 0x0F);
                hp = CalculateHpIv(attack, defense, special, speed);
                if (Generation == 1)
                {
                    types[0] = TypeData.GetName(GetData(currentPokemonOffset + 0x05));
                    types[1] = TypeData.GetName(GetData(currentPokemonOffset + 0x06));
                }
                else
                {
                    PokemonData.PokemonType[] typeData = pokemonData.GetPokemonType(GetData(currentPokemonOffset));
                    types[0] = typeData[0].ToString();
                    types[1] = typeData[1].ToString();
                }

                ivs = new IV
                {
                    HP = hp,
                    Attack = attack,
                    Defense = defense,
                    Speed = speed,
                    Special = special,

                };

                cursor = (ushort)(currentPokemonOffset + _offsets.BoxEv);
                ushort[] values = new ushort[5];
                for (ushort index = 0; index < 5; ++index)
                {
                    hexIn = GetData(cursor++, cursor++);
                    values[index] = (ushort)((hexIn[0] << 8) | hexIn[1]);
                }

                evs = new EVs
                {
                    HP = values[0],
                    Attack = values[1],
                    Defense = values[2],
                    Speed = values[3],
                    Special = values[4]
                };

                cursor = (ushort)(currentPokemonOffset + _offsets.PartyOtId);
                hexIn = GetData(cursor++, cursor++);
                int id = hexIn[0] << 8 | hexIn[1];

                cursor = (ushort)(currentPokemonOffset + 0x1B);
                if (Generation == 2)
                {
                    friendshipValue = GetData(cursor);
                }
                else
                {
                    friendshipValue = 0;
                }


                // Get OT name
                    otNameOffset = currentBoxOffset + _offsets.BoxOtName + (OT_NICK_NEXT_NAME_OFFSET * (i - 1));
                otName = TextEncoding.GetEncodedText(this, otNameOffset, 0x50, _offsets.TrainerNameSize);

                nickNameOffset = currentBoxOffset + _offsets.BoxNickname + (OT_NICK_NEXT_NAME_OFFSET * (i - 1));
                nickname = TextEncoding.GetEncodedText(this, nickNameOffset, 0x50, _offsets.TrainerNameSize);

                current = new Pokemon(name, level, ivs, new Stats(), evs, friendshipValue, otName, nickname, types, id, 1);
                boxPokemon.Add(current);
                currentPokemonOffset += _offsets.NextBoxPokemon;
                //otName.Clear();

            }

            return boxPokemon;
        }

        

        private List<Pokemon> GetPartyPokemon(bool sortByScore = false)
        {
            List<Pokemon> partyPokemon = new List<Pokemon>();
            Pokemon current;
            string speciesName;
            byte speciesId;
            ushort level;
            byte ad;
            byte ss;
            ushort attack;
            ushort defense;
            ushort speed;
            ushort special;
            ushort friendshipValue;
            IV ivs;
            Stats stats;
            EVs evs;
            ushort currentPokemonOffset = (ushort)(_offsets.PartySize + PARTY_SIZE_TO_FIRST_POKEMON_OFFSET);
            string otName;
            string nickname;
            string[] types = new string[2];
            int otNameOffset;
            int nickOffset;




            for (ushort i = 1; i <= GetPartySize(); ++i)
            {
                try
                {
                    speciesId = GetData(currentPokemonOffset);
                    speciesName = GetPartyPokemonName(i);
                    level = (ushort)GetData(currentPokemonOffset + _offsets.Level);
                    ad = GetData(currentPokemonOffset + _offsets.AttackDefense);
                    ss = GetData(currentPokemonOffset + _offsets.SpeedSpecial);
                    attack = (ushort)(ad >> 4);
                    defense = (ushort)(ad & 0x0F);
                    speed = (ushort)(ss >> 4);
                    special = (ushort)(ss & 0x0F);

                    if (Generation == 1)
                    {
                        types[0] = TypeData.GetName(GetData(currentPokemonOffset + _offsets.GenOneType1));
                        types[1] = TypeData.GetName(GetData(currentPokemonOffset + _offsets.GenOneType1 + 1));
                    }
                    else
                    {
                        PokemonData.PokemonType[] type = pokemonData.GetPokemonType(speciesId);
                        types[0] = type[0].ToString();
                        types[1] = type[1].ToString();
                    }
                    ivs = new IV
                    {
                        HP = CalculateHpIv(attack, defense, special, speed),
                        Attack = attack,
                        Defense = defense,
                        Speed = speed,
                        Special = special
                    };

                    ushort cursor = (ushort)(currentPokemonOffset + _offsets.Stats);
                    if (Generation == 1)
                    {
                        stats = new Stats
                        {
                            HP = (ushort)(GetData(cursor++) + GetData(cursor++)),
                            Attack = (ushort)(GetData(cursor++) + GetData(cursor++)),
                            Defense = (ushort)(GetData(cursor++) + GetData(cursor++)),
                            Speed = (ushort)(GetData(cursor++) + GetData(cursor++)),
                            SpecialAttack = (ushort)(GetData(cursor) + GetData(cursor + 1)),
                            SpecialDefense = (ushort)(GetData(cursor++) + GetData(cursor++))
                        };
                    }
                    else
                    {
                        stats = new Stats
                        {
                            HP = (ushort)(GetData(cursor++) + GetData(cursor++)),
                            Attack = (ushort)(GetData(cursor++) + GetData(cursor++)),
                            Defense = (ushort)(GetData(cursor++) + GetData(cursor++)),
                            Speed = (ushort)(GetData(cursor++) + GetData(cursor++)),
                            SpecialAttack = (ushort)(GetData(cursor++) + GetData(cursor++)),
                            SpecialDefense = (ushort)(GetData(cursor++) + GetData(cursor++))
                        };
                    }


                    cursor = (ushort)(currentPokemonOffset + _offsets.Ev);
                    ushort[] values = new ushort[5];
                    byte[] hexIn = new byte[2];
                    for (ushort index = 0; index < 5; ++index)
                    {
                        hexIn[0] = GetData(cursor++);
                        hexIn[1] = GetData(cursor++);
                        values[index] = (ushort)((hexIn[0] << 8) | hexIn[1]);
                    }
                    evs = new EVs
                    {
                        HP = values[0],
                        Attack = values[1],
                        Defense = values[2],
                        Speed = values[3],
                        Special = values[4]
                    };

                    cursor = (ushort)(currentPokemonOffset + _offsets.PartyOtId);
                    hexIn = GetData(cursor++, cursor++);
                    int id = hexIn[0] << 8 | hexIn[1];

                    cursor = (ushort)(currentPokemonOffset + 0x1B);
                    if (Generation == 2)
                    {
                        friendshipValue = GetData(cursor);
                    }
                    else
                    {
                        friendshipValue = 0;
                    }


                    otNameOffset = _offsets.PartySize + _offsets.PartyOtName + (OT_NICK_NEXT_NAME_OFFSET * (i - 1));
                    otName = TextEncoding.GetEncodedText(this, otNameOffset, 0x50, _offsets.TrainerNameSize);

                    nickOffset = _offsets.PartySize + _offsets.PartyNickName + (OT_NICK_NEXT_NAME_OFFSET * (i - 1));
                    nickname = TextEncoding.GetEncodedText(this, nickOffset, 0x50, _offsets.TrainerNameSize);

                    current = new Pokemon(speciesName, level, ivs, stats, evs, friendshipValue, otName, nickname, types, id, Generation);
                    partyPokemon.Add(current);
                    currentPokemonOffset += (ushort)_offsets.PartyNextPokemon; // increment by 44 bytes to get to next party pokemon
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            // Sort if requested
            if (sortByScore)
            {
                partyPokemon.Sort((x, y) => y.GetIvScore().CompareTo(x.GetIvScore()));
            }

            return partyPokemon;
        }

        private static ushort CalculateHpIv(ushort attack, ushort defense, ushort special, ushort speed)
        {
            ushort hpIv = 0;
            if (attack % 2 == 1)
            {
                hpIv += 8;
            }
            if (defense % 2 == 1)
            {
                hpIv += 4;
            }
            if (speed % 2 == 1)
            {
                hpIv += 2;
            }
            if (special % 2 == 1)
            {
                hpIv += 1;
            }
            return hpIv;
        }

        public void AddPartyPokemon(PokemonHexData data)
        {
            ushort partySize = GetPartySize();
            ushort partySizeOffset = _offsets.PartySize;
            ushort partyNextPokemonOffset = _offsets.PartyNextPokemon;
            if (partySize >= 6)
            {
                throw new ArgumentException("Error: Party is full. Must have an empty slot to add a pokemon to the party. Aborting.");
            }

            ushort slotNumber = (ushort)(partySize + 1);

            //PatchHexByte(0x01, partySizeOffset); debug only

            // 
            int insertOffset = partySizeOffset + PARTY_SIZE_TO_FIRST_POKEMON_OFFSET + (partyNextPokemonOffset * (slotNumber - 1));
            PatchHexBytes(data.data, insertOffset);

            PatchHexByte((byte)(partySize + 1), partySizeOffset);
            PatchHexByte(data.data[0], partySizeOffset + slotNumber);

            // Null terminator after party size (0xFF)
            PatchHexByte(0xFF, partySizeOffset + slotNumber + 1);

            insertOffset = partySizeOffset + _offsets.PartyOtName + (OT_NICK_NEXT_NAME_OFFSET * (slotNumber - 1));
            PatchHexBytes(data.otName, insertOffset);

            insertOffset = partySizeOffset + _offsets.PartyNickName + (OT_NICK_NEXT_NAME_OFFSET * (slotNumber - 1));
            PatchHexBytes(data.nickname, insertOffset);

            insertOffset = partySizeOffset + PARTY_SIZE_TO_FIRST_POKEMON_OFFSET + (partyNextPokemonOffset * (slotNumber - 1)) + _offsets.PartyOtId;
            PatchHexBytes(data.otId, insertOffset);


        }

        internal void WriteCSV(string filename, List<Pokemon> pokemon)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                if (Generation == 1)
                {
                    // Write the header row
                    writer.WriteLine("Species,Level,HP,Attack,Defense,Special Attack,Special Defense,Speed,IV Score,Percentile,Original Trainer,OT Id,Nickname");

                    foreach (Pokemon current in pokemon)
                    {
                        writer.WriteLine($"{current._speciesName},{current._level}," +
                        $"{current._ivs.HP},{current._ivs.Attack},{current._ivs.Defense}," +
                        $"{current._ivs.Special}, {current._ivs.Special},{current._ivs.Speed}," +
                        $"{current.GetIvScore()}, {current.getIvPercentile() / 100},{current._otName}," +
                        $"{current._otId:D5},{current._nickname}");
                    }
                }
                else
                {
                    writer.WriteLine("Species,Level,Held Item,HP,Attack,Defense,Special Attack,Special Defense,Speed,IV Score,Percentile,Original Trainer, OT Id,Nickname");
                    foreach (Pokemon current in pokemon)
                    {
                        writer.Write($"{current._speciesName},{current._level},");
                        if (current._heldItem == 0)
                        {
                            writer.Write("None,");
                        }
                        else
                        {
                            writer.Write($"{itemData.GetName(current._heldItem)},");
                        }
                        writer.Write($"{current._ivs.HP},{current._ivs.Attack},{current._ivs.Defense},");
                        writer.Write($"{current._ivs.Special},{current._ivs.Special},");
                        writer.Write($"{current._ivs.Speed},");
                        writer.Write($"{current.GetIvScore()},{current.getIvPercentile() / 100},");
                        writer.Write($"{current._otName},{current._otId:D5},");
                        writer.WriteLine($"{current._nickname}");
                    }


                }
            }
            Console.WriteLine("CSV File created.");
        }


        public static byte[] EncodeText(string text, byte terminator)
        {
            byte[] encodedText = new byte[text.Length + 1];
            for (int i = 0; i < text.Length; ++i)
            {
                encodedText[i] = TextEncoding.GetHexValue(text[i]);
            }

            encodedText[text.Length] = terminator;

            return encodedText;
        }

        public void EnableGSBallEvent()
        {
            const int GSFlagOffset = 0x3E3C;
            const int backupOffset = 0x3E44;
            const byte EnabledValue = 0x0B;
            PatchHexByte(EnabledValue, GSFlagOffset);
            PatchHexByte(EnabledValue, backupOffset);
        }



        public List<Item> GetBoxItems()
        {
            List<Item> items = new List<Item>();

            if (Generation == 2)
            {
                return items;
            }

            try
            {
                ushort bagQty = (ushort)GetData(boxItemsSizeOffset);
                int currentOffset = boxFirstItemOffset;
                byte itemHexCode;
                byte itemQty;
                string itemName;
                Item currentItem;

                for (ushort i = 1; i <= 50; i++)
                {
                    if (GetData(currentOffset) == 0xFF)
                    {
                        break;
                    }
                    itemHexCode = GetData(currentOffset++);
                    itemQty = GetData(currentOffset++);
                    itemName = itemData.GetName(itemHexCode);
                    currentItem = new Item(itemHexCode, (ushort)itemQty, itemName);
                    items.Add(currentItem);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return items;
        }

        public List<Item> GetTMPocketItems(int offset)
        {
            List<Item> tms = new List<Item>();

            byte[] tmOffsets = {
                0xBF, 0xC0, 0XC1, 0xC2, 0xC4,
                0xC5, 0xC6, 0xC7, 0xC8, 0xC9,
                0xCA, 0xCB, 0xCC, 0xCD, 0xCE,
                0xCF, 0xD0, 0xD1, 0xD2, 0xD3,
                0xD4, 0xD5, 0xD6, 0xD7, 0xD8,
                0xD9, 0xDA, 0xDB, 0xDD, 0xDE,
                0xDF, 0xE0, 0xE1, 0xE2, 0xE3,
                0xE4, 0xE5, 0xE6, 0xE7, 0xE8,
                0xE9, 0xEA, 0xEB, 0xEC, 0xED,
                0xEE, 0xEF, 0xF0, 0xF1, 0xF2,
                0xF3, 0xF4, 0xF5, 0xF6, 0xF7,
                0xF8, 0xF9, 0xFA, 0xFB, 0xFC,
                0xFD, 0xFE

            };

            try
            {
                byte itemQty;
                Item currentItem;

                for (ushort i = 0; i < 57; ++i)
                {
                    itemQty = GetData(offset++);
                    if (itemQty > 0)
                    {
                        currentItem = new Item(tmOffsets[i], itemQty, itemData.GetName(tmOffsets[i]));
                        tms.Add(currentItem);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return tms;
        }
        public List<Item> GetBagItems(int offset, ushort pocketMax, bool qtys = true)
        {
            List<Item> items = new List<Item>();


            try
            {
                ushort bagQty = GetData(offset);
                int currentOffset = offset + 0x01;
                byte itemHexCode;
                byte itemQty;
                string itemName;
                Item currentItem;

                for (ushort i = 1; i <= pocketMax; i++)
                {
                    if (GetData(currentOffset) == 0xFF)
                    {
                        break;
                    }
                    itemHexCode = GetData(currentOffset);
                    if (!qtys)
                    {
                        itemQty = 1;
                    }
                    else
                    {
                        itemQty = GetData(currentOffset + 0x01);
                    }

                    itemName = itemData.GetName(itemHexCode);
                    currentItem = new Item(itemHexCode, itemQty, itemName);
                    items.Add(currentItem);
                    if (!qtys)
                    {
                        currentOffset++;
                    }
                    else
                    {
                        currentOffset += 0x02;
                    }
                }


            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return items;
        }

        public void AddItemToBag(Item item)
        {
            ushort bagSizeOffset = _offsets.BagSize;
            ushort bagSize = (ushort)GetData(bagSizeOffset);

            if (bagSize >= 20)
            {
                Console.WriteLine("Unable to add item to bag. Bag is full. Aborting.");
                return;
            }

            if (item.quantity <= 0 || item.quantity > 255)
            {
                Console.WriteLine("Unable to add to bag. Invalid quantity. (Range 1 - 255) Aborting.");
                return;
            }

            // Increment bag size in save data
            PatchHexByte((byte)(bagSize + 1), bagSizeOffset);

            int offset = bagSizeOffset + 0x01 + (0x02 * bagSize);
            byte[] hexBytes = { item.hexCode, item.getQuantityHex(), 0xFF };
            PatchHexBytes(hexBytes, offset);

        }


        public string GenerateGameReport()
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendLine("-------------------");
            sb.AppendLine("Game Summary Report");
            sb.AppendLine("--------------------");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"{"Trainer Name: ",16} {GetTrainerName(),7}");
            sb.AppendLine();
            sb.AppendLine($"{"Pokemon Seen: ",16} {GetNumberSeen(),7:D3}");
            sb.AppendLine($"{"Pokemon Caught: ",16} {GetNumberOwned(),7:D3}");

            sb.AppendLine();

            Badges badges = GetBadges();

            sb.AppendLine(badges.getBadgesInfo(Generation));

            sb.AppendLine("-----------");
            sb.AppendLine("Party Info:");
            sb.AppendLine("___________");
            sb.AppendLine();

            sb.AppendLine(PartyPokemon.GetInfo());

            sb.AppendLine();
            sb.AppendLine(PcPokemon.GetPcPokemonInfo());

            sb.AppendLine();

            // sb.AppendLine("----------");
            // sb.AppendLine("Bag Items:");
            // sb.AppendLine("----------");
            // sb.AppendLine();


            sb.AppendLine(Items.GetInfo());


            return sb.ToString();
        }

        public ushort GetGender()
        {
            if (IsCrystal && GetData(genderAndShadowOffsetCrystal[0]) == 1)
            {
                return 1;
            }

            return 0;
        }

        public void UpdateMainChecksum(int checksum)
        {
            ushort checksumLocation = _offsets.MainChecksum;
            try
            {

                if (Generation == 1)
                {
                    byte checksumByte = (byte)(checksum & 0xFF);
                    PatchHexByte(checksumByte, checksumLocation);
                    Console.WriteLine($"Checksum updated to {checksumByte:X2} at {checksumLocation:X}");
                }
                else
                {
                    // For Gen 2, write the high byte first, then the low byte (big endian)
                    byte highByte = (byte)((checksum >> 8) & 0xFF);
                    byte lowByte = (byte)(checksum & 0xFF);
                    byte[] checksumBytes = new byte[] { highByte, lowByte };
                    PatchHexBytes(checksumBytes, checksumLocation);
                    Console.WriteLine($"Gen 2 Checksum updated to {highByte:X2}{lowByte:X2} at {checksumLocation:X}");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void UpdateMainChecksum(byte[] checksumBytes)
        {
            ushort checksumLocation = _offsets.MainChecksum;
            try
            {
                if (checksumBytes.Length != 2)
                {
                    throw new ArgumentException("Checksum byte array must be exactly 2 bytes long.");
                }

                PatchHexBytes(checksumBytes, checksumLocation);
                Console.WriteLine($"Checksum updated to {BitConverter.ToString(checksumBytes).Replace("-", "")} at {checksumLocation:X}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public void UpdateChecksum(int checksum, int location)
        {
            try
            {
                // Convert the hex string to bytes
                byte highByte = (byte)((checksum >> 8) & 0xFF);
                byte lowByte = (byte)(checksum & 0xFF);
                byte[] checksumBytes = new byte[] { highByte, lowByte };

                PatchHexBytes(checksumBytes, location);


            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public string GetTrainerID()
        {
            byte[] trainerIdHex = GetData(_offsets.TrainerId, _offsets.TrainerId + 1);
            ushort trainerId = (ushort)((trainerIdHex[0] << 8) | trainerIdHex[1]);
            return trainerId.ToString("D5");
        }

        public void SetTrainerName(byte[] name)
        {
            if (name.Length <= 0 || name.Length > 8 || name[name.Length - 1] != 0x50)
            {
                Console.WriteLine("Bad trainer name. Aborting.");
                return;
            }

            PatchHexBytes(name, _offsets.TrainerName);

        }

        public void SetRivalName(byte[] name)
        {
            if (name.Length <= 0 || name.Length > 8 || name[name.Length - 1] != 0x50)
            {
                Console.WriteLine("Bad rival name. Aborting.");
                return;
            }

            PatchHexBytes(name, _offsets.RivalName);
        }

        public void SetTrainerID(byte[] id)
        {
            if (id.Length != 2)
            {
                Console.WriteLine("Error: ID byte wrong size");
                return;
            }

            PatchHexBytes(id, _offsets.TrainerId);
        }

        public void SetMoney(int money)
        {
            if (money < 0)
            {
                return;
            }

            if (money > 999999)
            {
                money = 999999;
            }

            byte[] encodedMoney;

            if (Generation == 1)
            {
                encodedMoney = HexFunctions.ConvertIntToByteArray((uint)money);
                foreach (byte current in encodedMoney)
                {
                    Console.Write($"{current} ");
                }
            }
            else
            {
                encodedMoney = HexFunctions.IntToMoneyByte(money);
            }

            PatchHexBytes(encodedMoney, _offsets.Money);
        }

        public ushort GetGeneration() {
            return Generation;
    }

    }
}