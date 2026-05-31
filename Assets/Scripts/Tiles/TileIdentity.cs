using System;

namespace MahjongGame.Tiles
{
    public readonly struct TileIdentity : IEquatable<TileIdentity>
    {
        public int TileId { get; }

        public TileIdentity(int tileId)
        {
            TileId = tileId;
        }

        public bool IsValid => TileId >= 0;

        public bool Equals(TileIdentity other)
        {
            return TileId == other.TileId;
        }

        public override bool Equals(object obj)
        {
            return obj is TileIdentity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return TileId;
        }

        public override string ToString()
        {
            return "TileIdentity(" + TileId + ")";
        }
    }
}
