using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Cinemachine;
using Object = UnityEngine.Object;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector director;

    [Header("Player Lock")]
    [SerializeField] private bool lockPlayerDuringCutscene = true;

    [Header("On Finished")]
    public UnityEvent onCutsceneFinished;

    private PlayerMovment _playerMovment;
    private CinemachineCamera _playerFollowCamera;
    private CinemachineCamera[] _cutsceneCameras;

    private void Awake()
    {
        director.stopped += OnDirectorStopped;
    }

    private void Start()
    {
        FindPlayerFollowCamera();
        FindCutsceneCameras();
        SetCutsceneCamerasPriority(0);
        BindCinemachineBrainToTimeline();
        BindPlayerCameraToTimeline();
    }

    private void OnDestroy()
    {
        director.stopped -= OnDirectorStopped;
    }

    public void PlayCutscene()
    {
        if (lockPlayerDuringCutscene)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                _playerMovment = playerObj.GetComponent<PlayerMovment>();
            _playerMovment?.SetMovementLocked(true);
        }

        if (_playerFollowCamera != null)
            _playerFollowCamera.Priority = 0;

        director.Play();
    }

    public void SkipCutscene() => director.Stop();

    private void OnDirectorStopped(PlayableDirector d)
    {
        DisableCutsceneCameras();
        if (_playerFollowCamera != null) _playerFollowCamera.Priority = 10;
        _playerMovment?.SetMovementLocked(false);
        onCutsceneFinished?.Invoke();
    }

    private void FindPlayerFollowCamera()
    {
        var all = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var cam in all)
        {
            if (cam.Follow != null && cam.Follow.CompareTag("Player"))
            {
                _playerFollowCamera = cam;
                return;
            }
        }
    }

    private void FindCutsceneCameras()
    {
        var all = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var list = new List<CinemachineCamera>();
        foreach (var cam in all)
        {
            if (cam == _playerFollowCamera) continue;
            if (cam.gameObject.scene.name == "DontDestroyOnLoad") continue;
            list.Add(cam);
        }
        _cutsceneCameras = list.ToArray();
    }


    private void BindCinemachineBrainToTimeline()
    {
        var brain = FindFirstObjectByType<CinemachineBrain>(FindObjectsInactive.Include);
        if (brain == null) { Debug.LogWarning("[CutscenePlayer] CinemachineBrain not found!"); return; }

        foreach (var output in director.playableAsset.outputs)
        {
            if (output.outputTargetType != typeof(CinemachineBrain)) continue;
            director.SetGenericBinding(output.sourceObject, brain);
            return;
        }
    }

    private void BindPlayerCameraToTimeline()
    {
        if (_playerFollowCamera == null) return;
        var timeline = director.playableAsset as TimelineAsset;
        if (timeline == null) return;

        var staticSet = new HashSet<Object>();
        if (_cutsceneCameras != null)
            foreach (var c in _cutsceneCameras)
                if (c != null) staticSet.Add(c);

        foreach (var track in timeline.GetOutputTracks())
        {
            foreach (var clip in track.GetClips())
            {
                if (clip.asset is not CinemachineShot shot) continue;
                PropertyName expName = shot.VirtualCamera.exposedName;
                var existing = director.GetReferenceValue(expName, out bool valid);
                bool isStaticCam = valid && existing != null && staticSet.Contains(existing);
                if (!isStaticCam)
                    director.SetReferenceValue(expName, _playerFollowCamera);
            }
        }
    }


    private void SetCutsceneCamerasPriority(int priority)
    {
        if (_cutsceneCameras == null) return;
        foreach (var cam in _cutsceneCameras)
            if (cam != null) cam.Priority = priority;
    }

    private void DisableCutsceneCameras()
    {
        if (_cutsceneCameras == null) return;
        foreach (var cam in _cutsceneCameras)
            if (cam != null) cam.gameObject.SetActive(false);
    }
}
