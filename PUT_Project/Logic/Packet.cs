using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUT_Project.Logic
{
    [Serializable]
    class Packet
    {
        #region Private Fields
        DATAIDENTIFIER _dataID;
        GAMESTATE _state;
        string _name;
        string message;
        #endregion

        #region Public Fields
        public DATAIDENTIFIER DataID
        {
            get { return _dataID; }
            set { _dataID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public GAMESTATE GameState
        {
            get { return _state; }
            set { _state = value; }
        }
        #endregion

        public Packet()
        {
            _dataID = DATAIDENTIFIER.Null;
            _state = GAMESTATE.UNDEFINED;
            message = null;
            _name = null;
        }

        public Packet(byte[] dataStream)
        {
            this._dataID = (DATAIDENTIFIER)BitConverter.ToInt32(dataStream, 0);

            this._state = (GAMESTATE)BitConverter.ToInt32(dataStream, 4);
            

            int nameLength = BitConverter.ToInt32(dataStream, 8);
            int msgLength = BitConverter.ToInt32(dataStream, 12);

            if (nameLength > 0)
                this._name = Encoding.UTF8.GetString(dataStream, 16, nameLength);
            else
                this._name = null;

            if (msgLength > 0)
                this.message = Encoding.UTF8.GetString(dataStream, 16 + nameLength, msgLength);
            else
                this.message = null;
        }

        public byte[] GetDataStream()
        {
            List<byte> dataStream = new List<byte>();

            dataStream.AddRange(BitConverter.GetBytes((int)this._dataID));
            dataStream.AddRange(BitConverter.GetBytes((int)this._state));

            if (this._name != null)
                dataStream.AddRange(BitConverter.GetBytes(this._name.Length));
            else
                dataStream.AddRange(BitConverter.GetBytes(0));

            if (this.message != null)
                dataStream.AddRange(BitConverter.GetBytes(this.message.Length));
            else
                dataStream.AddRange(BitConverter.GetBytes(0));


            if (this._name != null)
                dataStream.AddRange(Encoding.UTF8.GetBytes(this._name));

            if (this.message != null)
                dataStream.AddRange(Encoding.UTF8.GetBytes(this.message));

            return dataStream.ToArray();
        }

    }

    public enum DATAIDENTIFIER
    {
        CHATMSG,
        INITIAL_CONNECTION_MSG,
        GAME_MSG,
        GAME_MOVE,
        STARTING_SETUP,
        Null
    }
}
