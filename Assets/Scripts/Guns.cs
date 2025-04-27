using System.Collections.Generic;
using UnityEngine;

public class IGuns
{
    public int GunID;
    public virtual void Update(float dt){;}
    public virtual void ShootGun(Vector3 position, Vector3 forward) {;}
    protected Animator handsAnimator { get; set; }
    protected HitResultsBuilder hitResultsBuilder { get; set; }
    protected GoalManager goalManager { get; set; }

    public IGuns(Animator animator, HitResultsBuilder resultsBuilder, GoalManager goals)
    {
        handsAnimator = animator;
        hitResultsBuilder = resultsBuilder;
        goalManager = goals;
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
    const float minimumCooldown = 0.2f;

    public Shotgun(Animator animator, HitResultsBuilder resultsBuilder, GoalManager goals) : base(animator, resultsBuilder, goals)
    {
        GunID = 0;
    }

    public override void Update(float dt)
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

    public SuperShotgun(Animator animator, HitResultsBuilder resultsBuilder, GoalManager goals) : base(animator, resultsBuilder, goals)
    {
        GunID = 1;
    }

    public override void Update(float dt)
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

                // Do shooting
                List<RaycastHit> hits = FireMultipleRays(position + forward, forward, 1, 50, out bool result);
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
                            if (numeric != EnemyNumeric.EnumCount && numeric < EnemyNumeric.Add)
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

    public MiniGun(Animator animator, HitResultsBuilder resultsBuilder, GoalManager goals) : base(animator, resultsBuilder, goals)
    {
        GunID = 1;
    }

    public override void Update(float dt)
    {
        timeSinceShot += dt;
    }

    public override void ShootGun(Vector3 position, Vector3 forward)
    {
        if (timeSinceShot > minimumCooldown)
        {
            // Not currently shooting
            if (handsAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "ChainGunShoot")
            {
                // Play Anim
                handsAnimator.SetTrigger("Shoot");

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