﻿using System;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour 
{
    public static LevelManager Instance { set; get; }

    public bool SHOW_COLLIDER = true; //$$

    //Level Spawn tools
    private const float DISTANCE_BEFORE_SPAWN = 100.0f;
    private const int INITIAL_SEGMENTS = 10;
    private const int INITIAL_TRANSITION_SEGMENTS = 2;
    private const int MAX_SEGMENTS = 15;
    private Transform cameraContainer;
    private int amoutOfActiveSegments;
    private int continousSegments;
    private int currentSpawnZ;
    private int currentLevel;
    private int y1, y2, y3;

    //List of pieces
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longblocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();
    [HideInInspector]
    public List<Piece> pieces = new List<Piece>(); //all active pieces

    //List of segments
    public List<Segment> availableSegments = new List<Segment>();
    public List<Segment> availableTransitions = new List<Segment>();
    public List<Segment> segments = new List<Segment>(); //all active segments

    //Gameplay
    private bool isMoving = false;

    private void Awake()
        {
        Instance = this;
        cameraContainer = Camera.main.transform; //you can use Camera.current.transform if you have more than one camera so it chooses the current one. You can only use .main if the camera is tagged MainCamera
        currentSpawnZ = 0;
        currentLevel = 0;
        }

    private void Start()
        {
        for (int i = 0; i < INITIAL_SEGMENTS; i++)
            {
            if(i < INITIAL_TRANSITION_SEGMENTS)
                {
                SpawnTransition();
                }
            else  GenerateSegment();
            }
        }

    private void Update()
        {
        if(currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN)
            {
            GenerateSegment();
            }

        if(amoutOfActiveSegments >= MAX_SEGMENTS)
            {
            segments[amoutOfActiveSegments - 1].DeSpawn();
            amoutOfActiveSegments--;
            }
        }

    void GenerateSegment()
        {
        SpawnSegment();

        if(UnityEngine.Random.Range(0f, 1f) < (continousSegments * 0.25f))
            {
            continousSegments = 0;
            SpawnTransition();
            }
        else
            {
            continousSegments++;
            }
        }

    void SpawnSegment()
        {
        List<Segment> possibleSeg = availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = UnityEngine.Random.Range(0, possibleSeg.Count);

        Segment s = GetSegment(id, false);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amoutOfActiveSegments++;
        s.Spawn();
        }

    void SpawnTransition()
        {
        List<Segment> possibleTransition = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = UnityEngine.Random.Range(0, possibleTransition.Count);

        Segment s = GetSegment(id, true);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amoutOfActiveSegments++;
        s.Spawn();
        }

    public Segment GetSegment(int id, bool transition)
        {
        Segment s = null;
        s = segments.Find(x => x.SegId == id && x.transition == transition && !x.gameObject.activeSelf);

        if(s == null)
            {
            GameObject go = Instantiate((transition) ? availableTransitions[id].gameObject : availableSegments[id].gameObject) as GameObject;
            s = go.GetComponent<Segment>();

            s.SegId = id;
            s.transition = transition;

            segments.Insert(0, s);
            }
        else
            {
            segments.Remove(s);
            segments.Insert(0, s);
            }
        return s;
        }

    public Piece GetPiece(PieceType pt, int visualIndex)
        {
       Piece p = pieces.Find(x => x.type == pt && x.visualIndex == visualIndex && !x.gameObject.activeSelf);
       
        if(p == null)
            {
            GameObject go = null;
            if(pt == PieceType.ramp)
                {
                go = ramps[visualIndex].gameObject;
                } else if(pt == PieceType.jump)
                {
                go = jumps[visualIndex].gameObject;
                } else if (pt == PieceType.longblock)
                {
                go = longblocks[visualIndex].gameObject;
                } else if(pt == PieceType.slide)
                {
                go = slides[visualIndex].gameObject;
                }

            go = Instantiate(go);
            p = go.GetComponent<Piece>();
            pieces.Add(p);
            }

        return p;
        }
    }
    
