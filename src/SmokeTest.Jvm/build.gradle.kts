plugins {
    kotlin("jvm") version "2.4.10"
    kotlin("plugin.serialization") version "2.4.10"
}

repositories {
    mavenCentral()
}

kotlin {
    jvmToolchain(21)
}

sourceSets {
    main {
        kotlin.srcDir("generated/kotlin")
        java.srcDir("generated/java")
    }
}

dependencies {
    implementation("net.typedrest:typedrest:0.33.2")
    implementation("net.typedrest:typedrest-serializers-jackson:0.33.1")
    implementation("org.jetbrains.kotlinx:kotlinx-serialization-json:1.11.0")
    compileOnly("org.jspecify:jspecify:1.0.1")
}

tasks.withType<org.jetbrains.kotlin.gradle.tasks.KotlinCompile>().configureEach {
    compilerOptions {
        allWarningsAsErrors.set(true)
    }
}

tasks.withType<JavaCompile>().configureEach {
    options.compilerArgs.addAll(listOf("-Xlint:all", "-Werror"))
}
