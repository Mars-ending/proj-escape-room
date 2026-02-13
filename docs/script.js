const container = document.getElementById('canvas-container');
const scene = new THREE.Scene();

const camera = new THREE.PerspectiveCamera(60, window.innerWidth / window.innerHeight, 0.1, 10000);
camera.position.set(0, 900, 0); 
camera.lookAt(0, 0, 0);

const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: false });
renderer.setSize(window.innerWidth, window.innerHeight);
renderer.setPixelRatio(window.devicePixelRatio);
renderer.toneMapping = THREE.ACESFilmicToneMapping;
renderer.toneMappingExposure = 1.3;
container.appendChild(renderer.domElement);

// === 辉光特效 ===
const renderScene = new THREE.RenderPass(scene, camera);
const bloomPass = new THREE.UnrealBloomPass(new THREE.Vector2(window.innerWidth, window.innerHeight), 1.5, 0.8, 0.9);
const composer = new THREE.EffectComposer(renderer);
composer.addPass(renderScene);
composer.addPass(bloomPass);

const ambientLight = new THREE.AmbientLight(0xffffff, 3.5); 
scene.add(ambientLight);

const tl = new THREE.TextureLoader();

// 太阳
const sunGeo = new THREE.SphereGeometry(20, 64, 64); 
const sunMat = new THREE.MeshBasicMaterial({ 
    map: tl.load('texture/sun.jpg'), 
    color: 0xffffff 
}); 
const sun = new THREE.Mesh(sunGeo, sunMat);
scene.add(sun);

// 太阳光晕
const glowGeo = new THREE.SphereGeometry(25, 64, 64);
const glowMat = new THREE.MeshBasicMaterial({
    color: 0xff4400, 
    transparent: true, 
    opacity: 0.45, 
    side: THREE.BackSide
});
const sunGlow = new THREE.Mesh(glowGeo, glowMat);
sunGlow.position.y = -2;
scene.add(sunGlow);

// 行星数据
const planets = [];
const planetData = [
    { name: "Mercury", size: 3.0,  distance: 45,  speed: 0.02,  color: 0xaaaaaa, tint: 0x888888, texture: 'texture/mercury.jpg' },
    { name: "Venus",   size: 4.5,  distance: 70,  speed: 0.015, color: 0xeecb8b, tint: 0x888888, texture: 'texture/venus.jpg' },
    { name: "Earth",   size: 4.8,  distance: 100, speed: 0.01,  color: 0x2233ff, tint: 0x999999, texture: 'texture/earth.jpg' },
    { name: "Mars",    size: 4.0,  distance: 130, speed: 0.008, color: 0xff4500, tint: 0x999999, texture: 'texture/mars.jpg' },
    { name: "Jupiter", size: 12.0, distance: 200, speed: 0.004, color: 0xd2b48c, tint: 0x666666, texture: 'texture/jupiter.jpg' }, 
    { name: "Saturn",  size: 10.0, distance: 280, speed: 0.003, color: 0xf4a460, tint: 0x666666, texture: 'texture/saturn.jpg', hasRing: true },
    { name: "Uranus",  size: 7.0,  distance: 360, speed: 0.002, color: 0x7fffd4, tint: 0x888888, texture: 'texture/uranus.jpg' },
    { name: "Neptune", size: 6.8,  distance: 420, speed: 0.001, color: 0x4169e1, tint: 0x888888, texture: 'texture/neptune.jpg' }
];

planetData.forEach(data => {
    const geo = new THREE.SphereGeometry(data.size, 64, 64);
    const mat = new THREE.MeshBasicMaterial({ 
        map: tl.load(data.texture, undefined, undefined, (err) => console.warn("Load failed")), 
        color: data.tint 
    });
    const mesh = new THREE.Mesh(geo, mat);
    // 初始位置计算
    const startAngle = Math.random() * Math.PI * 2;
    mesh.userData = { distance: data.distance, angle: startAngle, speed: data.speed };
    
    // 立即设置一次位置，防止初始画面行星都在原点
    mesh.position.x = Math.cos(startAngle) * data.distance;
    mesh.position.z = Math.sin(startAngle) * data.distance;

    if (data.hasRing) {
        const ringGeo = new THREE.RingGeometry(data.size * 1.4, data.size * 2.3, 128);
        var pos = ringGeo.attributes.position;
        var v3 = new THREE.Vector3();
        for (let i = 0; i < pos.count; i++){ v3.fromBufferAttribute(pos, i); ringGeo.attributes.uv.setXY(i, v3.length() < (data.size*1.8) ? 0 : 1, 1); }
        const ringMat = new THREE.MeshBasicMaterial({ 
            map: tl.load('texture/saturn_ring.png'), 
            color: 0x666666, side: THREE.DoubleSide, transparent: true, opacity: 0.8 
        });
        const ring = new THREE.Mesh(ringGeo, ringMat);
        ring.rotation.x = Math.PI / 2;
        mesh.add(ring);
    }

    const orbitGeo = new THREE.RingGeometry(data.distance - 0.4, data.distance + 0.4, 128);
    const orbitMat = new THREE.MeshBasicMaterial({ color: 0x88ccff, side: THREE.DoubleSide, transparent: true, opacity: 0.15 });
    const orbit = new THREE.Mesh(orbitGeo, orbitMat);
    orbit.rotation.x = Math.PI / 2;
    orbit.position.y = -5;
    scene.add(orbit);

    scene.add(mesh);
    planets.push(mesh);
});

const starGeo = new THREE.SphereGeometry(3000, 64, 64);
const starMat = new THREE.MeshBasicMaterial({ map: tl.load('texture/stars.jpg'), side: THREE.BackSide, color: 0xffffff });
const stars = new THREE.Mesh(starGeo, starMat);
scene.add(stars);

// === 【核心修改 1】摄像机配置 ===
// 我们不再写死 camPos，而是写 offset (相对行星的偏移量)
// === 【核心修改 1】摄像机配置（包含所有行星） ===
const slides = [
    { 
        id: 0, 
        // 封面：俯瞰全景
        type: 'absolute',
        camPos: { x: 0, y: 600, z: 800 }, 
        lookAt: { x: 0, y: 0, z: 0 }, 
        pauseOrbit: false 
    },
    { 
        id: 1, 
        // 背景故事：聚焦地球 (危机发生地)
        type: 'relative',
        targetPlanetIndex: 2, // Earth
        offset: { x: 25, y: 10, z: 25 }, 
        pauseOrbit: true
    },
    { 
        id: 2, 
        // 核心玩法：聚焦火星 (第一个任务点)
        type: 'relative',
        targetPlanetIndex: 3, // Mars
        offset: { x: 20, y: 10, z: 20 },
        pauseOrbit: true
    },
    { 
        id: 3, 
        // 加工流水线：聚焦木星 (巨大的资源工厂)
        type: 'relative',
        targetPlanetIndex: 4, // Jupiter
        offset: { x: 50, y: 30, z: 50 },
        pauseOrbit: true
    },
    { 
        id: 4, 
        // VR交互：聚焦土星 (需要精密操作的光环)
        type: 'relative',
        targetPlanetIndex: 5, // Saturn
        offset: { x: 50, y: 40, z: 50 },
        pauseOrbit: true
    },
    { 
        id: 5, 
        // 技术架构：聚焦天王星 (科技青色)
        type: 'relative',
        targetPlanetIndex: 6, // Uranus
        offset: { x: 30, y: 15, z: 30 }, 
        pauseOrbit: true
    },
    { 
        id: 6, 
        // 未来展望：聚焦海王星 (探索边缘)
        type: 'relative',
        targetPlanetIndex: 7, // Neptune
        offset: { x: 30, y: 15, z: 30 }, 
        pauseOrbit: true
    },
    { 
        id: 7, 
        // 结语：回到太阳 (希望与能量)
        type: 'absolute',
        camPos: { x: 0, y: 200, z: 300 }, 
        lookAt: { x: 0, y: 0, z: 0 }, 
        pauseOrbit: false 
    }
];

let currentSlide = 0;
let isOrbitPaused = false;
let isAnimating = false; // 防止狂点
const currentLookAt = new THREE.Vector3(0, 0, 0); 

// === 【核心修改 2】新的切换逻辑 ===
function changeSlide(dir) {
    if (isAnimating) return; // 动画中禁止点击

    const next = currentSlide + dir;
    if (next >= 0 && next < slides.length) {
        
        isAnimating = true; // 锁定
        
        document.getElementById(`slide-${currentSlide}`).classList.remove('active');
        currentSlide = next;
        document.getElementById(`slide-${currentSlide}`).classList.add('active');
        
        const target = slides[currentSlide];

        // 1. 设置是否暂停公转
        isOrbitPaused = target.pauseOrbit;

        // 2. 计算摄像机目标位置
        let targetPos = { x: 0, y: 0, z: 0 };
        let targetLook = { x: 0, y: 0, z: 0 };

        if (target.type === 'relative') {
            // === 动态追踪逻辑 ===
            const p = planets[target.targetPlanetIndex];
            
            // 获取行星当前的世界坐标
            const pX = p.position.x;
            const pY = p.position.y;
            const pZ = p.position.z;

            // 目标 LookAt 就是行星当前位置
            targetLook = { x: pX, y: pY, z: pZ };

            // 目标 CameraPos = 行星位置 + 偏移量
            // 注意：这里为了防止摄像机穿模，我们简单地加上偏移
            // 进阶做法是可以根据行星角度旋转偏移，但这里直接加就够用了
            targetPos = {
                x: pX + target.offset.x,
                y: pY + target.offset.y,
                z: pZ + target.offset.z
            };

        } else {
            // === 绝对坐标逻辑 (Slide 0, 1) ===
            targetPos = target.camPos;
            targetLook = target.lookAt;
        }

        // 3. 执行动画
        gsap.to(camera.position, { 
            x: targetPos.x, 
            y: targetPos.y, 
            z: targetPos.z, 
            duration: 2.5, 
            ease: "power2.inOut" 
        });

        gsap.to(currentLookAt, { 
            x: targetLook.x, 
            y: targetLook.y, 
            z: targetLook.z, 
            duration: 2.5, 
            ease: "power2.inOut",
            onComplete: () => {
                isAnimating = false; // 解锁
            }
        });
    }
}
window.changeSlide = changeSlide;

// 交互监听
window.addEventListener('wheel', (event) => {
    if (event.deltaY > 0) changeSlide(1);
    else changeSlide(-1);
});
window.addEventListener('keydown', (event) => {
    if (['ArrowRight', 'ArrowDown', ' '].includes(event.key)) changeSlide(1);
    else if (['ArrowLeft', 'ArrowUp'].includes(event.key)) changeSlide(-1);
});

function animate() {
    requestAnimationFrame(animate);

    planets.forEach(p => {
        // 只有不暂停时才更新位置
        if (!isOrbitPaused) {
            p.userData.angle += p.userData.speed * 0.5;
            p.position.x = Math.cos(p.userData.angle) * p.userData.distance;
            p.position.z = Math.sin(p.userData.angle) * p.userData.distance;
        }
        p.rotation.y += 0.005;
    });
    sun.rotation.y += 0.002;
    sunGlow.rotation.z -= 0.001;
    
    // GSAP 更新 currentLookAt，我们每一帧都 lookAt 它
    camera.lookAt(currentLookAt);
    composer.render();
}

window.addEventListener('resize', () => {
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
    composer.setSize(window.innerWidth, window.innerHeight);
});

animate();