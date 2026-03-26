using System.Collections;
using Runtime.Common.Services.LoadingCurtain;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Location
{
    public class LocationChanger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _fadeInDuration = 0.2f;
        [SerializeField] private float _stayBlackDuration = 0.5f;
        [SerializeField] private float _fadeOutDuration = 0.5f;
        
        // [Header("Audio")]
        // [SerializeField] private AudioClip _startSound;
        // [SerializeField] private AudioClip _endSound;

        private ILoadingCurtain _curtain;
        private CharacterController _playerController;
        // private AudioSource _audioSource;
        
        private bool _isProcessing;

        [Inject]
        public void Construct(ILoadingCurtain curtain)
        {
            _curtain = curtain;
        }

        public void Init(CharacterController playerController)
        {
            _playerController = playerController;
        }

        public void ChangeLocation(Transform targetPoint, bool needCurtain = true, bool needToDisableCC = true)
        {
            if (_isProcessing) 
                return;

            if (_playerController == null)
            {
                Debug.LogError("LocationChanger::ChangeLocation() PlayerController is null");
                return;
            }
            
            StartCoroutine(LocationChangeRoutine(targetPoint, needCurtain, needToDisableCC));
        }

        private IEnumerator LocationChangeRoutine(Transform target, bool needCurtain = true, bool needToDisableCC = true)
        {
            _isProcessing = true;
            
            if (needToDisableCC)
                _playerController.enabled = false;
            
            if (needCurtain)
            {
                _curtain.Show(_fadeInDuration, needText: false);
                yield return new WaitForSeconds(_fadeInDuration);
            }
            
            Teleport(target);
            
            if (needCurtain)
                yield return new WaitForSeconds(_stayBlackDuration);

            if (needCurtain)
            {
                _curtain.Hide(_fadeOutDuration, needText: false);
                yield return new WaitForSeconds(_fadeOutDuration);
            }

            _isProcessing = false;
            
            if (needToDisableCC)
                _playerController.enabled = true;
        }

        private void Teleport(Transform target)
        {
            _playerController.transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }
}