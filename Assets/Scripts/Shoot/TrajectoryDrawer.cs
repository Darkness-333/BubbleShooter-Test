using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TrajectoryDrawer {
    private BubbleCreator bubbleCreator;

    private LineRenderer lineRendererPrefab;
    private GameController gameController;

    private Vector3 dragStart;
    private Vector3 dragEnd;
    private bool isMaxStretch;
    private float maxStretch = 3f;
    private float scatterAngle = 5f;
    private int drawPointsCount = 35;
    private CancellationTokenSource cancellationTokenSource;
    private Bubble currentBubble;
    private LineRenderer trajectoryLine;
    private List<LineRenderer> scatterLines = new List<LineRenderer>();

    public bool canShoot { get; private set; }

    public TrajectoryDrawer(BubbleCreator bubbleCreator) {
        this.bubbleCreator = bubbleCreator;
        lineRendererPrefab = ServiceLocator.Get<LineRenderer>();
        gameController = ServiceLocator.Get<GameController>();

        canShoot = true;
        InitializeRenderers();

        currentBubble = bubbleCreator.PrepareNextBubble();
    }

    public async void ReloadInit() {
        StopWaiting();
        canShoot = false;
        currentBubble = bubbleCreator.PrepareNextBubble();
        await Task.Delay(10);
        canShoot = true;

    }

    public void InitializeRenderers() {

        trajectoryLine = Object.Instantiate(lineRendererPrefab);
        for (int i = 0; i < 2; i++) {
            LineRenderer scatterLine = Object.Instantiate(lineRendererPrefab);
            scatterLines.Add(scatterLine);
        }
    }

    public void HandleDragStart() {
        if (Input.GetMouseButtonDown(0)) {
            dragStart = GetMousePosition();
        }
    }

    public void HandleDragging() {
        if (Input.GetMouseButton(0)) {
            Vector3 currentEnd = GetMousePosition();
            Vector3 dragVector = currentEnd - dragStart;
            float stretch = Mathf.Clamp(dragVector.magnitude, 0, maxStretch);

            ShowTrajectory(currentEnd);
            HandleMaxStretch(stretch, currentEnd);
        }
    }

    private void HandleMaxStretch(float stretch, Vector3 currentEnd) {
        if (Mathf.Approximately(stretch, maxStretch)) {
            ShowScatterLines(currentEnd);
            isMaxStretch = true;
        }
        else {
            HideScatterLines();
            isMaxStretch = false;
        }
    }

    public void HandleShoot() {
        if (Input.GetMouseButtonUp(0)) {
            dragEnd = GetMousePosition();
            if ((dragStart - dragEnd).magnitude < .01f) return;
            currentBubble.AddRb().isKinematic = false;
            Vector3 force = CalculateForce(dragStart, dragEnd);

            if (isMaxStretch) {
                force = Quaternion.Euler(0, 0, Random.Range(-scatterAngle, scatterAngle)) * force;
                currentBubble.isMaxStretched = true;
            }

            ShootBall(force);
        }
    }

    private Vector3 GetMousePosition() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private Vector3 CalculateForce(Vector3 start, Vector3 end) {
        float forceCoeff = 10f;
        Vector3 direction = (start - end).normalized;
        float magnitude = (start - end).magnitude;
        magnitude = Mathf.Clamp(magnitude, 0, maxStretch);
        Vector3 force = direction * magnitude * forceCoeff;
        return force;
    }

    private void ShowTrajectory(Vector3 currentEnd) {
        float timeStep = 0.1f;

        Vector3 startPosition = currentBubble.transform.position;
        Vector3 force = CalculateForce(dragStart, currentEnd);
        Vector3 velocity = force / currentBubble.AddRb().mass;
        currentBubble.AddRb().isKinematic = true;

        Vector3[] points = CalculateTrajectoryPoints(startPosition, velocity, drawPointsCount, timeStep);
        SetRendererPoints(trajectoryLine, points);
    }

    private void SetRendererPoints(LineRenderer lineRenderer, Vector3[] points) {
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    private Vector3[] CalculateTrajectoryPoints(Vector3 startPosition, Vector3 velocity, int pointsCount, float timeStep) {
        Vector3[] points = new Vector3[pointsCount];
        for (int i = 0; i < pointsCount; i++) {
            float t = i * timeStep;
            points[i] = startPosition + velocity * t + 0.5f * (Vector3)Physics2D.gravity * t * t;

            if (points[i].y < -10) {
                System.Array.Resize(ref points, i + 1);
                break;
            }
        }
        return points;
    }

    private void ShowScatterLines(Vector3 currentEnd) {
        float timeStep = 0.1f;

        Vector3 startPosition = currentBubble.transform.position;
        Vector3 force = CalculateForce(dragStart, currentEnd);
        Vector3 velocity = force / currentBubble.AddRb().mass;

        Vector3 leftVelocity = Quaternion.Euler(0, 0, scatterAngle) * velocity;
        Vector3 rightVelocity = Quaternion.Euler(0, 0, -scatterAngle) * velocity;

        Vector3[] leftPoints = CalculateTrajectoryPoints(startPosition, leftVelocity, drawPointsCount, timeStep);
        Vector3[] rightPoints = CalculateTrajectoryPoints(startPosition, rightVelocity, drawPointsCount, timeStep);

        SetRendererPoints(scatterLines[0], leftPoints);
        SetRendererPoints(scatterLines[1], rightPoints);
    }

    private void HideScatterLines() {
        foreach (var scatterLine in scatterLines) {
            scatterLine.positionCount = 0;
        }
    }

    private void ShootBall(Vector3 force) {
        currentBubble.AddRb().AddForce(force, ForceMode2D.Impulse);

        trajectoryLine.positionCount = 0;
        HideScatterLines();

        cancellationTokenSource = new CancellationTokenSource();
        WaitForBubbleToStopAndPrepareNext(cancellationTokenSource.Token);
    }


    private async void WaitForBubbleToStopAndPrepareNext(CancellationToken token) {
        canShoot = false;
        while (currentBubble.gameObject.activeSelf) {
            if (token.IsCancellationRequested) return;

            if (!currentBubble.haveRb) break;

            await Task.Yield();
        }

        while (!gameController.endWaitFall) {
            if (token.IsCancellationRequested) return;

            await Task.Yield();
        }
        canShoot = true;

        currentBubble = bubbleCreator.PrepareNextBubble();
    }

    public void StopWaiting() {
        cancellationTokenSource?.Cancel();
    }

}
