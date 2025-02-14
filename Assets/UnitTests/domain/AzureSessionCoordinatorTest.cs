using NSubstitute;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class AzureSessionCoordinatorTest
{
    IAnchorsRepository anchorsRepository = Substitute.For<IAnchorsRepository>();
    IObjectsCreator objectsCreator = Substitute.For<IObjectsCreator>();
    IAnchorLocator anchorLocator = Substitute.For<IAnchorLocator>();
    IStartAzureSession startAzureSession = Substitute.For<IStartAzureSession>();
    IAnchorCreator saveAnchor = Substitute.For<IAnchorCreator>();
    IGameObjectEditor gameObjectEditor = Substitute.For<IGameObjectEditor>();


    AzureSessionCoordinator subject;

    [SetUp]
    public void setUp()
    {
        var go = new GameObject();
        go.AddComponent<AzureSessionCoordinator>();
        subject = go.GetComponent<AzureSessionCoordinator>();
        subject.Construct(
            anchorsRepository,
            anchorLocator,
            objectsCreator,
            startAzureSession,
            saveAnchor,
            gameObjectEditor
        );
    }

    [TestFixture]
    public class NoAnchorsToLocate : AzureSessionCoordinatorTest
    {
        [SetUp]
        public new void setUp()
        {
            anchorsRepository.getAnchorsIds().Returns(new List<string>());
            subject.Start();
        }


        [UnityTest]
        public IEnumerator startsAzureSession()
        {
            while (!subject.isStarted)
            {
                yield return null;
            }
            startAzureSession.Received().invoke();
        }

        [UnityTest]
        public IEnumerator doesNotStartLocatingAzureAnchors()
        {
            while (!subject.isStarted)
            {
                yield return null;
            }
            anchorLocator.DidNotReceive().startLocatingAzureAnchors(Arg.Any<string[]>());
        }
    }


    [TestFixture]
    public class AnchorsToLocate : AzureSessionCoordinatorTest
    {
        private static readonly string anchorId1 = "12345789";

        private readonly List<string> anchorsIds = new List<string> { anchorId1 };

        [SetUp]
        public new void setUp()
        {
            anchorsRepository.getAnchorsIds().Returns(anchorsIds);
            subject.Start();
        }

        [TestFixture]
        public class LocatesAnchors : AnchorsToLocate
        {
            [UnityTest]
            public IEnumerator startsAzureSession()
            {
                while (!subject.isStarted)
                {
                    yield return null;
                }
                startAzureSession.Received().invoke();
            }

            [UnityTest]
            public IEnumerator startsLocatingAzureAnchors()
            {
                while (!subject.isStarted)
                {
                    yield return null;
                }
                anchorLocator.Received().startLocatingAzureAnchors(Arg.Any<string[]>());
            }
        }

        [TestFixture]
        public class AnchorLocated : AnchorsToLocate
        {
            GameObject mockedAnchorGO = new GameObject();
            GameObject prefabAnchor = new GameObject();
            static Vector3 mockedVector3 = Vector3.one;
            IAnchorLocator.CloudAnchorLocatedArgs mockedArgs = new IAnchorLocator.CloudAnchorLocatedArgs(
                        new Pose(Vector3.right, Quaternion.identity),
                        0,
                        anchorId1,
                        mockedVector3
                    );
            [SetUp]
            public new void setUp()
            {
                objectsCreator.allMachines = new GameObject[] { prefabAnchor };

                objectsCreator.createNewMachineWithGO(0).Returns(mockedAnchorGO);
                anchorLocator.CloudAnchorLocated +=
                    Raise.Event<CloudAnchorLocated>(mockedArgs);
            }

            [Test]
            public void setsNewGameObjectPose()
            {
                gameObjectEditor.Received().setPose(mockedAnchorGO, mockedArgs.pose, mockedVector3);
            }

            [Test]
            public void setsNewGameObjectNameAsIdentifier()
            {
                gameObjectEditor.Received().setName(mockedAnchorGO, mockedArgs.identifier);
            }

            [Test]
            public void createsNewAnchorNativeAnchor()
            {
                saveAnchor.Received().createNativeAnchor(mockedAnchorGO);
            }

            [Test]
            public void savesAnchorIdentifier()
            {
                anchorsRepository.Received().addAnchor(
                    Arg.Is<IAnchorsRepository.AnchorGameObject>(
                        x => x.identifier == mockedArgs.identifier
                        )
                    );
            }

            [Test]
            public void savesAnchorGamoObject()
            {
                anchorsRepository.Received().addAnchor(
                    Arg.Is<IAnchorsRepository.AnchorGameObject>(
                            x => x.gameObject == mockedAnchorGO
                        )
                    );
            }
        }
    }
}
