using UnityEngine;
using UnityEngine.UI;


public class Pourable : Dragable {
    [SerializeField] float pourRate;
    [SerializeField] float reserveMaxFill;
    [SerializeField] float secondaryPourReserveMaxFill;

    [SerializeField] bool pourClockwise;

    [SerializeField] GameObject uprightImage;
    [SerializeField] Image secondaryPourReserve;

    [SerializeField] GameObject sidewaysImage;
    [SerializeField] Image pourReserve;

    [SerializeField] GameObject pourLine;

    Quaternion startAngle;
    Quaternion secondaryStartAngle;
    Quaternion pourLineStartAngle;

    protected override void Start() {
        base.Start();

        secondaryStartAngle = transform.rotation;
        startAngle = sidewaysImage.gameObject.transform.rotation;
        pourLineStartAngle = pourLine.gameObject.transform.rotation;
    }

    public float pourReserveTick() {
        float fillDelta = (Time.deltaTime * pourRate);
        float relativeFillDelta = fillDelta * reserveMaxFill;

        if (pourReserve.fillAmount - relativeFillDelta < 0) {
            pourLine.SetActive(false);
            return 0;
        }

        pourReserve.fillAmount -= relativeFillDelta;
        float angleDelta = (pourClockwise ? -1 : 1) * (relativeFillDelta * 90);

        sidewaysImage.transform.rotation *= Quaternion.AngleAxis(angleDelta, Vector3.forward);
        pourLine.transform.rotation *=
            Quaternion.AngleAxis(-angleDelta, Vector3.forward);

        return fillDelta;
    }

    public void StartPour() {
        pourLine.SetActive(true);
        sidewaysImage.SetActive(true);
        uprightImage.SetActive(false);
    }

    public void StopPour() {
        secondaryPourReserve.fillAmount =
            (pourReserve.fillAmount / reserveMaxFill) * secondaryPourReserveMaxFill;

        pourLine.SetActive(false);
        sidewaysImage.SetActive(false);
        uprightImage.SetActive(true);
    }

    public void resetPourReserve() {
        pourReserve.fillAmount = reserveMaxFill;
        secondaryPourReserve.fillAmount = secondaryPourReserveMaxFill;

        transform.rotation = secondaryStartAngle;
        sidewaysImage.transform.rotation = startAngle;
        pourLine.transform.rotation = pourLineStartAngle;
    }
}
