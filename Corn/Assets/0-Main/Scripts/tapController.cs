using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tapController : MonoBehaviour
{
    [SerializeField] private int totalNumberOfTaps = 2;
    [SerializeField] private int numberOfOpenTaps = 0;
    [SerializeField] private float waterChangeSpeedMultiplier = 10f;
    [SerializeField] private Transform tapWaterEffect;
    private AudioSource _myAS;
    [SerializeField] private AudioClip[] _audioClips;
    private Vector3 fullTapWaterScale;


    // Start is called before the first frame update
    void Start()
    {
        fullTapWaterScale = tapWaterEffect.localScale;
        _myAS = GetComponent<AudioSource>();
        tapWaterEffect.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfOpenTaps <= 0)
            return;


        Vector3 currentWaterScale = fullTapWaterScale * ((float) numberOfOpenTaps / totalNumberOfTaps);
        currentWaterScale.y = fullTapWaterScale.y;
        tapWaterEffect.localScale = Vector3.Lerp(tapWaterEffect.localScale, currentWaterScale,
            Time.deltaTime * waterChangeSpeedMultiplier);

        var tapWaterAS = tapWaterEffect.GetComponent<AudioSource>();
        tapWaterAS.volume = ((float) numberOfOpenTaps / totalNumberOfTaps); 
    }

    public void OpenTap()
    {
        if (numberOfOpenTaps == 0)
        {
            var particleSys = tapWaterEffect.GetComponent<ParticleSystem>();
            _myAS.PlayOneShot(_audioClips[0]); //open tap clip
            tapWaterEffect.gameObject.SetActive(true);

            if (!particleSys.isPlaying)
                particleSys.Play();

            var tapWaterAS = tapWaterEffect.GetComponent<AudioSource>();

            if (!tapWaterAS.isPlaying)
                tapWaterAS.Play();
        }

        numberOfOpenTaps++;
    }

    public void CloseTap()
    {
        numberOfOpenTaps--;

        if (numberOfOpenTaps == 0)
        {
            tapWaterEffect.GetComponent<AudioSource>().Stop();
            _myAS.PlayOneShot(_audioClips[1]); //close tap clip

            var particleSys = tapWaterEffect.GetComponent<ParticleSystem>();
            particleSys.Stop();
            particleSys.Clear();

            tapWaterEffect.gameObject.SetActive(false);
        }
    }
}