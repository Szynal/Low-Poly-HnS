using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static partial class FE_SaveLoadSystem
{
#if UNITY_SWITCH && !UNITY_EDITOR
    public static void SaveGame(FE_SaveFile _save, string _fileName)
    {
       /* //First, we need to make connection with user, so we can save game in his files
        nn.account.Account.Initialize();

        nn.account.Uid _userID = new nn.account.Uid();
        nn.account.UserHandle _userHandle = new nn.account.UserHandle();

        nn.account.Account.TryOpenPreselectedUser(ref _userHandle);
        nn.account.Account.GetUserId(ref _userID, _userHandle);

        nn.Result _operationResult = nn.fs.SaveData.Mount("saveData", _userID);
        _operationResult.abortUnlessSuccess();*/

        string _filePath = FE_SwitchIOManager.Instance.MountName + ":/" + _fileName;
        nn.fs.FileHandle _handle = new nn.fs.FileHandle();

        //Then we format the save class
        BinaryFormatter _formatter = new BinaryFormatter();
        byte[] _data;
        using (MemoryStream _memStream = new MemoryStream())
        {
            _formatter.Serialize(_memStream, _save);
            _memStream.Close();
            _data = _memStream.GetBuffer();
        }

        // Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();

        //And then we save the formatted save into a new file
        nn.Result _operationResult = nn.fs.File.Delete(_filePath);
        if(nn.fs.FileSystem.ResultPathNotFound.Includes(_operationResult) == false)
        {
            _operationResult.abortUnlessSuccess();
        }

        _operationResult = nn.fs.File.Create(_filePath, _data.LongLength);
        _operationResult.abortUnlessSuccess();

        _operationResult = nn.fs.File.Open(ref _handle, _filePath, nn.fs.OpenFileMode.Write);
        _operationResult.abortUnlessSuccess();

        _operationResult = nn.fs.File.Write(_handle, 0, _data, _data.LongLength, nn.fs.WriteOption.Flush);
        _operationResult.abortUnlessSuccess();

        nn.fs.File.Close(_handle);
        _operationResult = nn.fs.FileSystem.Commit(FE_SwitchIOManager.Instance.MountName);
        _operationResult.abortUnlessSuccess();

        // Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
    }

    public static FE_SaveFile GetSavedFile(string _fileName)
    {
        FE_SaveFile _retFile = null;

        nn.fs.FileHandle _handle = new nn.fs.FileHandle();
        string _filePath = FE_SwitchIOManager.Instance.MountName + ":/" + _fileName;

        nn.fs.EntryType _type = 0;
        nn.Result _operationResult = nn.fs.FileSystem.GetEntryType(ref _type, _filePath);
        
        if(nn.fs.FileSystem.ResultPathNotFound.Includes(_operationResult) == true)
        {
            return null;
        }
        _operationResult.abortUnlessSuccess();

        _operationResult = nn.fs.File.Open(ref _handle, _filePath, nn.fs.OpenFileMode.Read);
        _operationResult.abortUnlessSuccess();

        long _fileSize = 0;
        _operationResult = nn.fs.File.GetSize(ref _fileSize, _handle);
        _operationResult.abortUnlessSuccess();

        byte[] _saveData = new byte[_fileSize];
        _operationResult = nn.fs.File.Read(_handle, 0, _saveData, _fileSize);
        _operationResult.abortUnlessSuccess();

        BinaryFormatter _formatter = new BinaryFormatter();
        using (MemoryStream _memStream = new MemoryStream(_saveData))
        {
            // SceneLoader.Instance.LoadFromSave((FE_Save)(_formatter.Deserialize(_memStream)));
            _retFile = (FE_SaveFile)(_formatter.Deserialize(_memStream));
        }

        nn.fs.File.Close(_handle);
        return _retFile;
    }

#else
    public static void SaveGame(FE_SaveFile _save, string _fileName)
    {
        BinaryFormatter _formatter = new BinaryFormatter();
        FileStream _file = File.Create(Application.persistentDataPath + "/" + _fileName);
        _formatter.Serialize(_file, _save);
        _file.Close();
    }

    public static FE_SaveFile GetSavedFile(string _fileName)
    {
        FE_SaveFile _retFile = null;

        if (File.Exists(Application.persistentDataPath + "/" + _fileName))
        {
            BinaryFormatter _formatter = new BinaryFormatter();
            FileStream _file = File.Open(Application.persistentDataPath + "/" + _fileName, FileMode.Open);
            _retFile = (FE_SaveFile) _formatter.Deserialize(_file);
            _file.Close();
        }

        return _retFile;
    }
#endif
}