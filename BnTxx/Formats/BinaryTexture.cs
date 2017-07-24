﻿using BnTxx.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BnTxx.Formats
{
    public class BinaryTexture : IList<Texture>
    {
        public List<Texture> Textures;

        private PatriciaTree NameTree;

        public Texture this[int Index]
        {
            get => Textures[Index];
            set => Textures[Index] = value;
        }

        public int Count => Textures.Count;

        public bool IsReadOnly => false;

        /* Initialization and loading */

        public BinaryTexture()
        {
            Textures = new List<Texture>();

            NameTree = new PatriciaTree();
        }

        public BinaryTexture(Stream Data) : this(new BinaryReader(Data)) { }

        public BinaryTexture(BinaryReader Reader) : this()
        {
            string BnTxSignature = Reader.ReadString(8);

            CheckSignature("BNTX", BnTxSignature);

            int    DataLength     = Reader.ReadInt32();
            ushort ByteOrderMark  = Reader.ReadUInt16();
            ushort FormatRevision = Reader.ReadUInt16();
            int    NameAddress    = Reader.ReadInt32();
            int    StringsAddress = Reader.ReadInt32() >> 16;
            int    RelocAddress   = Reader.ReadInt32();
            int    FileLength     = Reader.ReadInt32();

            ReadBinaryTextureInfo(Reader);
        }

        private void ReadBinaryTextureInfo(BinaryReader Reader)
        {
            string NXSignature = Reader.ReadString(4);

            CheckSignature("NX  ", NXSignature);

            uint TexturesCount   = Reader.ReadUInt32();
            long InfoPtrsAddress = Reader.ReadInt64();
            long DataBlkAddress  = Reader.ReadInt64();
            long DictAddress     = Reader.ReadInt64();
            uint StrDictLength   = Reader.ReadUInt32();

            Reader.BaseStream.Seek(DictAddress, SeekOrigin.Begin);

            NameTree = new PatriciaTree(Reader);

            for (int Index = 0; Index < TexturesCount; Index++)
            {
                Reader.BaseStream.Seek(InfoPtrsAddress + Index * 8, SeekOrigin.Begin);
                Reader.BaseStream.Seek(Reader.ReadInt64(),          SeekOrigin.Begin);

                long ppp = Reader.BaseStream.Position;

                string BRTISignature = Reader.ReadString(4);

                CheckSignature("BRTI", BRTISignature);

                int    BRTILength0   = Reader.ReadInt32();
                long   BRTILength1   = Reader.ReadInt64();
                uint   Unknown10     = Reader.ReadUInt32();
                ushort Unknown14     = Reader.ReadUInt16();
                ushort Mipmaps       = Reader.ReadUInt16();
                uint   Unknown18     = Reader.ReadUInt32();
                uint   Format        = Reader.ReadUInt32();
                uint   Unknown20     = Reader.ReadUInt32();
                int    Width         = Reader.ReadInt32();
                int    Height        = Reader.ReadInt32();
                uint   Unknown2C     = Reader.ReadUInt32();
                int    FacesCount    = Reader.ReadInt32();
                int    ChannelsCount = Reader.ReadInt32();
                uint   Unknown38     = Reader.ReadUInt32();
                uint   Unknown3C     = Reader.ReadUInt32();
                uint   Unknown40     = Reader.ReadUInt32();
                uint   Unknown44     = Reader.ReadUInt32();
                uint   Unknown48     = Reader.ReadUInt32();
                uint   Unknown4C     = Reader.ReadUInt32();
                int    DataLength    = Reader.ReadInt32();
                int    BlockSize     = Reader.ReadInt32();
                int    ChannelTypes  = Reader.ReadInt32();
                uint   Unknown5C     = Reader.ReadUInt32();
                long   NameAddress   = Reader.ReadInt64();
                long   Unknown68     = Reader.ReadInt64();
                long   PtrsAddress   = Reader.ReadInt64();

                Reader.BaseStream.Seek(NameAddress, SeekOrigin.Begin);

                string Name = Reader.ReadShortString();

                Reader.BaseStream.Seek(PtrsAddress,        SeekOrigin.Begin);
                Reader.BaseStream.Seek(Reader.ReadInt64(), SeekOrigin.Begin);

                byte[] Data = Reader.ReadBytes(DataLength);

                TextureFormatType FormatType    = (TextureFormatType)((Format >> 8) & 0xff);
                TextureFormatVar  FormatVariant = (TextureFormatVar) ((Format >> 0) & 0xff);

                Textures.Add(new Texture()
                {
                    Name          = Name,
                    Width         = Width,
                    Height        = Height,
                    BlockSize     = BlockSize,
                    Mipmaps       = Mipmaps,
                    Data          = Data,
                    FormatType    = FormatType,
                    FormatVariant = FormatVariant
                });
            }
        }

        private void CheckSignature(string Expected, string Actual)
        {
            if (Actual != Expected)
            {
                throw new InvalidSignatureException(Expected, Actual);
            }
        }

        /* Public facing methods */

        public IEnumerator<Texture> GetEnumerator()
        {
            return Textures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Texture item)
        {
            Textures.Add(item);
        }

        public void Clear()
        {
            Textures.Clear();
        }

        public bool Contains(Texture item)
        {
            return Textures.Contains(item);
        }

        public void CopyTo(Texture[] array, int arrayIndex)
        {
            Textures.CopyTo(array, arrayIndex);
        }

        public int IndexOf(Texture item)
        {
            return Textures.IndexOf(item);
        }

        public void Insert(int index, Texture item)
        {
            Textures.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Textures.RemoveAt(index);
        }

        bool ICollection<Texture>.Remove(Texture item)
        {
            return Textures.Remove(item);
        }
    }
}