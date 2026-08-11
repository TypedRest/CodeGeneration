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
    implementation("net.typedrest:typedrest:0.33.0")
    implementation("net.typedrest:typedrest-serializers-jackson:0.32.0")

    // The @Serializable and @SerialName annotations the Kotlin generator emits. TypedRest depends on
    // kotlinx-serialization only as `implementation`, so it does not reach a consumer's compile classpath and has
    // to be declared here; the kotlin("plugin.serialization") plugin adds the compiler plugin but no dependency.
    implementation("org.jetbrains.kotlinx:kotlinx-serialization-json:1.9.0")

    // Carries the @Nullable annotations the Java generator emits, so Kotlin sees real nullability
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
