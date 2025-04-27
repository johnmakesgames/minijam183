using System.Collections.Generic;
using UnityEngine;

public class IGuns
{
    public int GunID;
    public virtual void Update(float dt, Vector3 position, Vector3 forward) {;}
    public virtual void ShootGun(Vector3 position, Vector3 forward) {;}
    protected Animator handsAnimator { get; set; }
    protected HitResultsBuilder hitResultsBuilder { get; set; }
    protected GoalManager goalManager { get; set; }
    protected AudioSource gunShootAudioSource;
    protected AudioSource bfgAudiOSource;

    public IGuns(Animator animator, HitResultsBuilder resultsBuilder, GoalManager goals, AudioSource shootAudioSource, AudioSource bfgAudioSource)
    {
        handsAnimator = animator;
        hitResultsBuilder = resultsBuilder;
        goalManager = goals;
        gunShootAudioSource = shootAudioSource;
        bfgAudiOSource = bfgAudioSource;
    }

    protected List<RaycastHit> FireMultipleRays(Vector3 centralPosition, Vector3 direciton, int verticalRayCount, int horizontalRayCount, out bool result)
    {
        result = false;
        List<RaycastHit> hits = new List<RaycastHit>();
        int halfHorizontalRayCount = horizontalRayCount / 2;
        for (int j = -halfHorizontalRayCount; j < horizontalRayCount - halfHorizontalRayCount; j++)
        {
            int halfVerticalRayCount = verticalRayCount / 2;
            for (int i = -halfVerticalRayCount; i < verticalRayCount - halfVerticalRayCount; i++)
            {
                Vector3 origin = centralPosition;
                origin.y += (float)i / 5;
                origin.x += (float)j / 10;

                Ray ray = new Ray();
                ray.origin = origin;
                ray.direction = direciton;
                RaycastHit hitResult = new RaycastHit();

                bool thisOneHit = false;
                if (Physics.Raycast(ray, out hitResult))
                {
                    hits.Add(hitResult);
                    result = true;
                    thisOneHit = true;
                }

                Debug.DrawRay(origin, direciton * 1000, (thisOneHit) ? Color.green : Color.red, 5);
            }
        }

        return hits;
    }
}

public class Shotgun : IGuns
{
    float timeSinceShot = 0;
    const float minimumCooldown = 0.4f;

    public Shotgun(Animator animator, HitResultsBuilder resultsBuilder, GoalManager goals, AudioSource shootAudioSource, AudioSource bfgAudioSource) : base(animator, resultsBuilder, goals, shootAudioSource, bfgAudioSource)
    {
        GunID = 0;
    }

    public override void Update(float dt, Vector3 position, Vector3 forward)
    {
        timeSinceShot += dt;
    }

    public override void ShootGun(Vector3 position, Vector3 forward)
    {
        if (timeSinceShot > minimumCooldown)
        {
            // Not currently shooting
            if (handsAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "PlayerShoot")
            {
                // Play Anim
                handsAnimator.SetTrigger("Shoot");
                gunShootAudioSource.Play();

                // Do shooting
                List<RaycastHit> hits = FireMultipleRays(position + forward, forward, 80, 3, out bool result);
                if (result)
                {
                    bool sorted = true;
                    do
                    {
                        sorted = true;
                        for (int i = 1; i < hits.Count; i++)
                        {
                            if (hits[i - 1].distance > hits[i].distance)
                            {
                                RaycastHit hit1 = hits[i - 1];
                                hits[i - 1] = hits[i];
                                hits[i] = hit1;
                                sorted = false;
                            }
                        }
                    }
                    while (!sorted);

                    foreach (var hit in hits)
                    {
                        if (hit.transform.gameObject.GetComponent<EnemyController>())
                        {
                            Debug.Log("Hit Enemy");

                            EnemyNumeric numeric = hit.transform.gameObject.GetComponent<EnemyController>().DoDamage(1);
                            if (numeric != EnemyNumeric.EnumCount)
                            {
                                // Do builder here
                                Debug.Log("Killed Enemy");
                                if (numeric != EnemyNumeric.Equals)
                                {
                                    hitResultsBuilder.Symbol(numeric);
                                }
                                else
                                {
                                    goalManager.CheckGoal(hitResultsBuilder.Result());
                                }
                            }

                            break;
                        }
                    }
                }

                timeSinceShot = 0;
            }
        }
    }
}

public class SuperShotgun : IGuns
{
    float timeSinceShot = 0;
    const float minimumCooldown = 0.5f;

    public SuperShotgun(Animator animator, HitResultsBuilder resultsBuilder, GoalManager goals, AudioSource shootAudioSource, AudioSource bfgAudioSource) : base(animator, resultsBuilder, goals, shootAudioSource, bfgAudioSource)
    {
        GunID = 1;
    }

    public override void Update(float dt, Vector3 position, Vector3 forward)
    {
        timeSinceShot += dt;
    }

    public override void ShootGun(Vector3 position, Vector3 forward)
    {
        if (timeSinceShot > minimumCooldown)
        {
            // Not currently shooting
            if (handsAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "SuperShotgun")
            {
                // Play Anim
                handsAnimator.SetTrigger("Shoot");
                gunShootAudioSource.Play();
                gunShootAudioSource.PlayDelayed(0.05f);

                // Do shooting
                List<RaycastHit> hits = FireMultipleRays(position + forward, forward, 1, 50, out bool result);
                if (result)
                {
                    HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

                    foreach (var hit in hits)
                    {
                        if (hit.transform.gameObject.GetComponent<EnemyController>() && !hitEnemies.Contains(hit.transform.gameObject))
                        {
                            hitEnemies.Add(hit.transform.gameObject);
                            EnemyNumeric numeric = hit.transform.gameObject.GetComponent<EnemyController>().DoDamage(1);
                            if (numeric != EnemyNumeric.EnumCount)
                            {
                                // Do builder here
                                if (numeric != EnemyNumeric.Equals)
                                {
                                    hitResultsBuilder.Symbol(numeric);
                                }
                                else
                                {
                                    goalManager.CheckGoal(hitResultsBuilder.Result());
                                }
                            }
                        }
                    }
                }

                timeSinceShot = 0;
            }
        }
    }
}

public class MiniGun : IGuns
{
    float timeSinceShot = 0;
    const float minimumCooldown = 0.1f;

    public MiniGun(Animator animator, HitResultsBuilder resultsBuilder, GoalManager goals, AudioSource shootAudioSource, AudioSource bfgAudioSource) : base(animator, resultsBuilder, goals, shootAudioSource, bfgAudioSource)
    {
        GunID = 2;
    }

    public override void Update(float dt, Vector3 position, Vector3 forward)
    {
        timeSinceShot += dt;
    }

    public override void ShootGun(Vector3 position, Vector3 forward)
    {
        if (timeSinceShot > minimumCooldown)
        {
            // Not currently shooting
            {
                // Play Anim
                handsAnimator.SetTrigger("Shoot");
                gunShootAudioSource.Play();

                // Do shooting
                List<RaycastHit> hits = FireMultipleRays(position + forward, forward, 50, 2, out bool result);
                if (result)
                {
                    bool sorted = true;
                    do
                    {
                        sorted = true;
                        for (int i = 1; i < hits.Count; i++)
                        {
                            if (hits[i - 1].distance > hits[i].distance)
                            {
                                RaycastHit hit1 = hits[i - 1];
                                hits[i - 1] = hits[i];
                                hits[i] = hit1;
                                sorted = false;
                            }
                        }
                    }
                    while (!sorted);

                    foreach (var hit in hits)
                    {
                        if (hit.transform.gameObject.GetComponent<EnemyController>())
                        {
                            Debug.Log("Hit Enemy");

                            EnemyNumeric numeric = hit.transform.gameObject.GetComponent<EnemyController>().DoDamage(1);
                            if (numeric != EnemyNumeric.EnumCount)
                            {
                                // Do builder here
                                Debug.Log("Killed Enemy");
                                if (numeric != EnemyNumeric.Equals)
                                {
                                    hitResultsBuilder.Symbol(numeric);
                                }
                                else
                                {
                                    goalManager.CheckGoal(hitResultsBuilder.Result());
                                }
                            }

                            break;
                        }
                    }
                }

                timeSinceShot = 0;
            }
        }
    }
}

public class BFG : IGuns
{
    float timeSinceShot = 0;
    const float minimumCooldown = 10.0f;
    bool pendingShoot = false;

    public BFG(Animator animator, HitResultsBuilder resultsBuilder, GoalManager goals, AudioSource shootAudioSource, AudioSource bfgAudioSource) : base(animator, resultsBuilder, goals, shootAudioSource, bfgAudioSource)
    {
        GunID = 3;
    }

    public override void Update(float dt, Vector3 position, Vector3 forward)
    {
        timeSinceShot += dt;

        if (pendingShoot)
        {
            if (timeSinceShot >= 0.42f)
            {
                // Do shooting
                List<RaycastHit> hits = FireMultipleRays(position + forward, forward, 1, 300, out bool result);
                if (result)
                {
                    HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

                    foreach (var hit in hits)
                    {
                        if (hit.transform.gameObject.GetComponent<EnemyController>() && !hitEnemies.Contains(hit.transform.gameObject))
                        {
                            hitEnemies.Add(hit.transform.gameObject);
                            EnemyNumeric numeric = hit.transform.gameObject.GetComponent<EnemyController>().DoDamage(1);
                            if (numeric != EnemyNumeric.EnumCount)
                            {
                                // Do builder here
                                if (numeric != EnemyNumeric.Equals)
                                {
                                    hitResultsBuilder.Symbol(numeric);
                                }
                                else
                                {
                                    goalManager.CheckGoal(hitResultsBuilder.Result());
                                }
                            }
                        }
                    }
                }

                pendingShoot = false;
            }
        }
    }

    public override void ShootGun(Vector3 position, Vector3 forward)
    {
        if (timeSinceShot > minimumCooldown)
        {
            // Not currently shooting
            if (handsAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "BFGShoot")
            {
                // Play Anim
                handsAnimator.SetTrigger("Shoot");
                bfgAudiOSource.PlayDelayed(0.4f);
                pendingShoot = true;
                timeSinceShot = 0;
            }
        }
    }
}